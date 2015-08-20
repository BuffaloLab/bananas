using UnityEngine;
using System.Collections;

public class AvatarControls_Training : Avatar{
	public TrainingState state;

	public delegate void OnFoodCollision();
	public OnFoodCollision OnFoodCollisionDelegate;

	public delegate void OnFoodLineup();
	public OnFoodLineup OnFoodLineupDelegate;

	int layerMask = 1 << 8; //FOOD is layer 8--> mask only looks at this layer;
	RaycastHit hit;
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Food")) 
		{
			OnFoodCollisionDelegate(); //All functions that subscribe to this event will be called.
			reward.RewardAndFreeze(1);
			Destroy(other.gameObject);
		}
	}

	public override void GetInput(){
		//Before really checking input, need to see if the last movement put us in line with the fruit

		if (Physics.Raycast (transform.position, transform.forward, out hit, Mathf.Infinity,layerMask)) {
			OnFoodLineupDelegate();
		}

		float verticalAxisInput = Input.GetAxis ("Vertical");
		float horizontalAxisInput = Input.GetAxis ("Horizontal");
		switch (state.Move){
		case TrainingState.Movement.forward:
				if ( verticalAxisInput > 0.01f) //for any hardware calibration errors
				{
					GetComponent<Rigidbody>().velocity = transform.forward*verticalAxisInput*driveSpeed; //should have no deltaTime framerate component -- given the frame, you should always be moving at a speed directly based on the input
					//NOTE: potential problem with this method: joysticks and keyboard input will have different acceleration calibration.
					
				}
				else{
					GetComponent<Rigidbody>().velocity = Vector3.zero;
				}
			break;
		case TrainingState.Movement.left:
				if (horizontalAxisInput < 0.01f) { //for any hardware calibration errors
					
					//Turn( horizontalAxisInput*RotationSpeed*(Time.deltaTime) ); 
					GetComponent<Rigidbody> ().angularVelocity = Vector3.up * horizontalAxisInput * RotationSpeed;
					//Debug.Log("horizontal axis ANG VEL = " + GetComponent<Rigidbody>().angularVelocity);
				}
				else {
					GetComponent<Rigidbody> ().angularVelocity = Vector3.zero * horizontalAxisInput * RotationSpeed;
				}
				
			break;
		case TrainingState.Movement.right:
				if (horizontalAxisInput > 0.01f) { //for any hardware calibration errors
					
					//Turn( horizontalAxisInput*RotationSpeed*(Time.deltaTime) ); 
					GetComponent<Rigidbody> ().angularVelocity = Vector3.up * horizontalAxisInput * RotationSpeed;
					//Debug.Log("horizontal axis ANG VEL = " + GetComponent<Rigidbody>().angularVelocity);
				}
				else {
					GetComponent<Rigidbody> ().angularVelocity = Vector3.zero * horizontalAxisInput * RotationSpeed;
				}

			break;
		case TrainingState.Movement.turn:
				if (Mathf.Abs (horizontalAxisInput) > 0.01f) { //for any hardware calibration errors
					
					//Turn( horizontalAxisInput*RotationSpeed*(Time.deltaTime) ); 
					GetComponent<Rigidbody> ().angularVelocity = Vector3.up * horizontalAxisInput * RotationSpeed;
					//Debug.Log("horizontal axis ANG VEL = " + GetComponent<Rigidbody>().angularVelocity);
				}
				else {
					GetComponent<Rigidbody> ().angularVelocity = Vector3.zero * horizontalAxisInput * RotationSpeed;
				}
			break;
		case TrainingState.Movement.full:
			if ( verticalAxisInput > 0.01f) //for any hardware calibration errors
			{
				GetComponent<Rigidbody>().velocity = transform.forward*verticalAxisInput*driveSpeed; //should have no deltaTime framerate component -- given the frame, you should always be moving at a speed directly based on the input
				//NOTE: potential problem with this method: joysticks and keyboard input will have different acceleration calibration.
				
			}
			else{
				GetComponent<Rigidbody>().velocity = Vector3.zero;
			}
		
			if (Mathf.Abs (horizontalAxisInput) > 0.01f) { //for any hardware calibration errors
				
				//Turn( horizontalAxisInput*RotationSpeed*(Time.deltaTime) ); 
				GetComponent<Rigidbody> ().angularVelocity = Vector3.up * horizontalAxisInput * RotationSpeed;
				//Debug.Log("horizontal axis ANG VEL = " + GetComponent<Rigidbody>().angularVelocity);
			}
			else {
				GetComponent<Rigidbody> ().angularVelocity = Vector3.zero * horizontalAxisInput * RotationSpeed;
			}
			break;
		}
	}
}