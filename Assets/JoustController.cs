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

	public float maxFlyingSpeed = 35.0f;
	public float maxGroundSpeed = 25.0f;
	public float flyingAcceleration = 15.0f;
	public float groundedAcceleration = 8.0f;
	public float flappingPower = 15.0f;
	public float chargeSpeed = 250.0f;

	[SerializeField]
	private float currentSpeed = 0.0f;
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
	private Vector2 _birdVelocity = Vector2.zero;
	[SerializeField]
	private float glideGravityScale = 0.3f;
	[SerializeField]
	private float groundLinearDrag = 0.0f;
	[SerializeField]
	private float flyingLinearDrag = 0.0f;
	[Range(0,1)]
	public float flyingDragCoef = 0.5f;


	void Awake() 
	{
		// Cache the sprite transform
		myTransform = GetComponent<Transform>();
		myRidgidBody = myTransform.GetComponent<Rigidbody2D>();

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
		if( flappingCDTimer > 0 && flappingCDTimer < flappingCDTime )
		{
			flappingCDTimer += Time.deltaTime; // increment the timer
		}
		else if( flappingCDTimer > flappingCDTime )
		{
			flappingCDTimer = 0.0f; // reset the timer
			hasFlapped = false;
		}

		if( chargeCDTimer > 0 && chargeCDTimer < chargeCDTime )
		{
			chargeCDTimer += Time.deltaTime;
		}
		else if( chargeCDTimer > chargeCDTime )
		{
			chargeCDTimer = 0.0f;
		}

		float horzPoll = Input.GetAxis ("Horizontal");
		facing = ( horzPoll > 0.0f ) ? 1.0f : (( horzPoll < 0.0f ) ? -1.0f : facing );

		if( Input.GetButtonDown("Fire1") && flappingCDTimer == 0.0f )
		{
			_birdVelocity.y += flappingPower * Time.deltaTime;
			flappingCDTimer += Time.deltaTime;
			isGliding = true;
			hasFlapped = true;

			_birdVelocity.x += horzPoll * flyingAcceleration * Time.deltaTime; // apply horizontal while flying only on a flap
		}

		if( Input.GetButtonUp("Fire1") || isGrounded )
		{
			isGliding = false;
		}

		if( Input.GetButtonDown ("Fire2") && !isGrounded && chargeCDTimer == 0.0f 
		   && facing == direction )
		{
			_birdVelocity.x += direction * chargeSpeed;
			chargeCDTimer += Time.deltaTime;
		}


		// check to see if the bird is grounded if so add the grounded speed to velocity vector
		if( isGrounded )
		{
			_birdVelocity.x += horzPoll * groundedAcceleration * Time.deltaTime;
		} else
		{
			//_birdVelocity.x += horzPoll * flyingAcceleration * Time.deltaTime;
		}

		// check to see if the direction has changed
		if( myRidgidBody.velocity.x < 0 )
		{
			direction = -1; 
		} else if( myRidgidBody.velocity.x > 0)
		{
			direction = 1; 
		}
	
		//myTransform.localScale = new Vector3( facing, 0.0f, 0.0f );

	}// End Update()

	void FixedUpdate () 
	{
		myRidgidBody.velocity += _birdVelocity;

		if( isGliding )
		{
			myRidgidBody.gravityScale = 0.2f;
		} else
		{
			myRidgidBody.gravityScale = 1.0f;
		}


		// preform limit clamping on the velocity vector
		_birdVelocity.y = 0.0f;
		if( isGrounded ) // If the bird is on the ground clamp to the max ground velocity
		{
			// apply the proper linear drag for being on the ground
			myRidgidBody.drag = groundLinearDrag;

			if( myRidgidBody.velocity.x > maxGroundSpeed )
			{
				_birdVelocity.x = maxGroundSpeed;
				myRidgidBody.velocity += _birdVelocity;
			}
			else if( myRidgidBody.velocity.x < -maxGroundSpeed )
			{
				_birdVelocity.x = -maxGroundSpeed;
				myRidgidBody.velocity += _birdVelocity;
			}
		} else if( !isGrounded ) // if the bird is in the air clamp to the max flying velocity
		{
			// apply the proper linear drag for being in the air
			myRidgidBody.drag = flyingLinearDrag;

			if( myRidgidBody.velocity.x > maxFlyingSpeed )
			{
				_birdVelocity.x = maxFlyingSpeed;
				myRidgidBody.velocity += _birdVelocity;
			}
			else if( _birdVelocity.x < -maxFlyingSpeed )
			{
				_birdVelocity.x = -maxFlyingSpeed;
				myRidgidBody.velocity += _birdVelocity;
			}
		}
	
		_birdVelocity = Vector2.zero;
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
