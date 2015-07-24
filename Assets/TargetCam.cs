using UnityEngine;
using System.Collections;

public class TargetCam : MonoBehaviour 
{
	public Transform target = null;
	public float minFollowSpeed = 10.0f;
	private Transform camPos = null;
	private Rigidbody2D velTarget = null;

	[SerializeField]
	private float followSpeed = 3.0f;
	private Vector3 _Pos = Vector3.zero; 

	void Awake() 
	{
		camPos = GetComponent<Transform>();
		velTarget = target.GetComponent<Rigidbody2D>();
	}
	

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		followSpeed = Mathf.Lerp(followSpeed, velTarget.velocity.magnitude, 0.1f );

		//followSpeed = velTarget.velocity.magnitude * 2.0f;
		followSpeed = (followSpeed > minFollowSpeed) ? followSpeed : minFollowSpeed;
	

		_Pos = Vector3.MoveTowards( camPos.position, target.position, followSpeed * Time.deltaTime );
		_Pos.z = camPos.position.z;
		camPos.position = _Pos;
			
	}
}
