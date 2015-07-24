using UnityEngine;
using System.Collections;

public class JoustController : MonoBehaviour {
	private Transform myTransform = null; 
	private Rigidbody2D myRidgidBody = null;
	
	[SerializeField]
	private bool isGrounded = false;
	[SerializeField]
	private bool isGliding = false; 
	[SerializeField]
	private bool hasFlapped = false; 

	public bool overrideMaxSpeed = false; 
	
	public float maxFlyingVelocity = 8.0f;
	public float flyingX_Acceleration = 50.0f;
	public float flyingY_Acceleration = 120.0f;
	public float flyingChargeAcceleration = 30.0f;
	public float maxGroundVelocity = 5.0f;
	public float groundedAcceleration = 8.0f;

	
	[SerializeField]
	private float flappingCDTime = 0.2f;
	[SerializeField]
	private float flappingCDTimer = 0.0f;
	[SerializeField]
	private float chargeCDTime = 0.5f;
	[SerializeField]
	private float chargeCDTimer = 0.0f;
	[SerializeField]
	private float direction = 1.0f;
	public float facing = 1.0f;

	[SerializeField]
	private float _axisTolerance = 0.1f;
	[SerializeField]
	private Vector2 _birdAcceleration = Vector2.zero;
	[SerializeField]
	private Vector2 _birdVel = Vector2.zero;
	[SerializeField]
	private float glideGravityScale = 0.3f;
	[SerializeField]
	public float groundLinearDrag = 0.0f;
	[Range(0,1)]
	public float flyingDragCoef = 0.5f;
	[SerializeField]
	private float flyingLinearDrag = 0.0f;
	[Range(0,1)]
	public float flyingDragWhileCharging = 0.8f;
	[Range(0,1)]
	public float goundDragWhileOverVMax = 0.5f;


	void Awake() 
	{
		// Cache the frequently called components. 
		myTransform = GetComponent<Transform>();
		myRidgidBody = myTransform.GetComponent<Rigidbody2D>();

		// compute the drag for the ground and air.
		groundLinearDrag = myRidgidBody.drag;
		flyingLinearDrag = groundLinearDrag*flyingDragCoef;

	}


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		// update cooldown timers
		// check to see if the flapping Cooldown has been activated and if it less then the cooldown time.

		// if flap timer is greater then zero then it has been started 
		// if it is less then the Cool Down Period we need to increment the timer. 
		if( flappingCDTimer > 0 && flappingCDTimer < flappingCDTime )
		{
			flappingCDTimer += Time.deltaTime; // increment the timer
		}
		else if( flappingCDTimer > flappingCDTime )
		{
			// the flap timer has exceeded the threshold reset associated variables.
			flappingCDTimer = 0.0f;
			hasFlapped = false; 
		}

		// if charge timer is greater then zero then it has been started 
		// if it is less then the Cool Down Period we need to increment the timer. 
		if( chargeCDTimer > 0 && chargeCDTimer < chargeCDTime )
		{
			chargeCDTimer += Time.deltaTime;
		}
		else if( chargeCDTimer > chargeCDTime )
		{
			// the charge timer has exceeded the threshold reset associated variables.
			chargeCDTimer = 0.0f;
		}

		// Grab and store the Horizontal Axis position. 
		float horzPoll = Input.GetAxis ("Horizontal");
		// Normalize the horzPoll so that deadzone reads as 0;
		horzPoll = ( horzPoll > _axisTolerance ) ? horzPoll : (( horzPoll < -_axisTolerance ) ? horzPoll : 0 );

		// Determine the facing of the sprite based on the Horizontal Axis Poll. 
		// If it is in the dead zone then set to the previous facing. 
		facing = ( horzPoll > 0 ) ? 1.0f : (( horzPoll < 0 ) ? -1.0f : facing );

		// check to see if flap button has been pressed and flap is not on cooldown.
		if( Input.GetButtonDown("Fire1") && flappingCDTimer == 0.0f )
		{
			// Since we are in the update function, it may process several times before the fixed update, so we are accumilating/agregating 
			// the x and y contributions to the Acceleration
			_birdAcceleration.y += flyingY_Acceleration * Time.deltaTime; // apply the vertical acceleration to the bird
			_birdAcceleration.x += horzPoll * flyingX_Acceleration * Time.deltaTime; // apply the horizontal acceleration to the bird for flying. 

			// Start the flap cooldown timer
			flappingCDTimer += Time.deltaTime;  

			// set the applicable status flags
			isGliding = true;
			hasFlapped = true;
		}

		// check to see if the flap button has been released or the bird is on the ground
		// if so then the bird is nolonger gliding. 
		if( Input.GetButtonUp("Fire1") || isGrounded )
		{
			isGliding = false;
		}

		// check to see if the charge button has been pressed, and the bird is not on the ground
		// and the charge cooldown timer is has not been started. Then make sure the bird is facing
		// the direction it has velocity in (it can not charge in reverse). 
		if( Input.GetButtonDown ("Fire2") && !isGrounded && chargeCDTimer == 0.0f 
		   && facing == direction )
		{
			_birdAcceleration.x += direction * flyingChargeAcceleration; // apply the horizontal acceleration to the bird for charging.

			// Start the Charge CoolDown Timer. 
			chargeCDTimer += Time.deltaTime; 

			// Set the state to bypass the acceleration limit check. 
			overrideMaxSpeed = true;
		}


		// check to see if the bird is on the ground.
		if( isGrounded )
		{
			// apply the proper linear drag for being on the ground
			myRidgidBody.drag = groundLinearDrag;
			// since the bird is on the ground and horzPoll is 0 unless the player is moving it 
			// to the right or left, so we apply the horizontal acceleration for walking to the bird. 
			_birdAcceleration.x += horzPoll * groundedAcceleration * Time.deltaTime;
		} else
		{
			// apply the proper linear drag for being in the air
			myRidgidBody.drag = flyingLinearDrag;
		}

		// check to see if the direction the bird is moving has changed
		if( myRidgidBody.velocity.x < 0 )
		{
			direction = -1; 
		} else if( myRidgidBody.velocity.x > 0)
		{
			direction = 1; 
		}

	}// End Update()

	void FixedUpdate () 
	{
	
		// Check to see if the bird is gliding and falling if so set the gravity scale
		// so that it falls slower, the wings give it boyencey
		if( isGliding && myRidgidBody.velocity.y < 0 )
		{
			myRidgidBody.gravityScale = glideGravityScale;
		} else
		{
			myRidgidBody.gravityScale = 1.0f;
		}

		// Check to make sure that we are not suppose to allow Excessive Acceleration over Velocity Limits
		if( !overrideMaxSpeed )
		{
			// If on the ground and we are over our Velocity Limits then: 
			// 1. zero out the _birdVelocity calculated in Update function. 
			// 2. calculate an acceleration vector that will bring the velocity back to the Max Velocity Limit over time. 
			if( isGrounded )
			{
				if( myRidgidBody.velocity.x > maxGroundVelocity )
				{
					_birdAcceleration = -Vector2.Lerp( myRidgidBody.velocity - new Vector2(maxGroundVelocity, myRidgidBody.velocity.y), Vector2.zero, goundDragWhileOverVMax);
				} else if( myRidgidBody.velocity.x < -maxGroundVelocity )
				{
					_birdAcceleration = -Vector2.Lerp( myRidgidBody.velocity - new Vector2(-maxGroundVelocity, myRidgidBody.velocity.y), Vector2.zero, goundDragWhileOverVMax);
				}
			}
			else 
			{
				if( myRidgidBody.velocity.x > maxFlyingVelocity )
				{
					_birdAcceleration = -Vector2.Lerp( myRidgidBody.velocity - new Vector2(maxFlyingVelocity, myRidgidBody.velocity.y), Vector2.zero, flyingDragWhileCharging);
				} else if( myRidgidBody.velocity.x < -maxFlyingVelocity )
				{
					_birdAcceleration = -Vector2.Lerp( myRidgidBody.velocity - new Vector2(-maxFlyingVelocity, myRidgidBody.velocity.y), Vector2.zero, flyingDragWhileCharging);
				}
			}
		}

		// Apply the Aggregated/Accumilated acceleration to the bird.
		myRidgidBody.velocity += _birdAcceleration;

		//TODO -- remove the _birdVel varaible it is only being used so I can track the Rigidbody Velocity in the Editor
		// while developing the game. 
		_birdVel = myRidgidBody.velocity;

		// We have used the aggregated acceleration and need to reset it to zero for next pass. 
		_birdAcceleration = Vector2.zero;
		// Acceleration that would by pass the Maximum limit has been applied reset the Override flag. 
		overrideMaxSpeed = false;
	} // End FixedUpdate()

	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == "Ground")
		{
			isGrounded = true;
		}
		
	} // End OnCollisionEnter2D

	void OnCollisionExit2D(Collision2D coll) {
		if (coll.gameObject.tag == "Ground")
		{
			isGrounded = false;
		}
		
	} // End OnCollisionExit2D
}
