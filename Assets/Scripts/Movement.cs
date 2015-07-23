using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	Animator _animator = null; 

	void Awake () {
		_animator = this.GetComponent<Animator>();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		int currentAnimationState = _animator.GetInteger ("WyvernAnimationState");
		float _dir = Input.GetAxis("Horizontal");

		// If Wyvern is Flying Left and Play goes Right
		if( _dir >= 0 && currentAnimationState == 4 )
		{
			_animator.SetInteger("WyvernAnimationState",3); // Set the Wyvern to Hover Left
		}
		// If Wyvern is Hovering Left and Play goes Right
		if( _dir > 0 && (currentAnimationState == 3 || currentAnimationState == 2) )
		{
			_animator.SetInteger("WyvernAnimationState",5); // Turn Wyvern Right
		}
		// If the Wyvern is Turning Right
		if( _dir > 0 && currentAnimationState == 5 )
		{
			_animator.SetInteger("WyvernAnimationState",0); // Fly to the Right
		}

		// If the Wyvern is Hovering Right and pressing Right
		if( _dir > 0 && currentAnimationState == 0)
		{
			_animator.SetInteger("WyvernAnimationState",1); // Fly to the Right
		}

		// If Wyvern is Flying Right and Play goes Left
		if( _dir <= 0 && currentAnimationState == 1 )
		{
			_animator.SetInteger("WyvernAnimationState",0); // Set the Wyvern to Hover Right
		}
		// If Wyvern is Hovering Right and Play goes Left
		if( _dir < 0 && currentAnimationState == 0 )
		{
			_animator.SetInteger("WyvernAnimationState",2); // Turn Wyvern Left
		}
		// If the Wyvern is Turning Left
		if( _dir < 0 && (currentAnimationState == 3 || currentAnimationState == 2) )
		{
			_animator.SetInteger("WyvernAnimationState",4); // Fly to the Right
		}


	}
}
