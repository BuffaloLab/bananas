using UnityEngine;
using System.Collections;

public class AvatarControls : MonoBehaviour{

	GameObject collisionObject;

	public bool ShouldLockControls = false;
	public float driveSpeed = 5.0f;


	public float RotationSpeed = 1;


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(!ShouldLockControls){
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY; // TODO: on collision, don't allow a change in angular velocity?
			
			GetInput ();
		}
		else{
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
		}
	}

	void FixedUpdate(){

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Food")) 
		{
			Destroy(other.gameObject);
			//other.gameObject.SetActive(false);
			//other.gameObject.tag = "Eaten";
			//print (GameObject.FindGameObjectsWithTag ("Food").Length);
			//count = count+1;
		}
	}

	void GetInput()
	{
		float verticalAxisInput = Input.GetAxis ("Vertical");

		if ( Mathf.Abs(verticalAxisInput) > 0.01f) //for any hardware calibration errors
		{
			GetComponent<Rigidbody>().velocity = transform.forward*verticalAxisInput*driveSpeed; //should have no deltaTime framerate component -- given the frame, you should always be moving at a speed directly based on the input
																											//NOTE: potential problem with this method: joysticks and keyboard input will have different acceleration calibration.

		}
		else{
			GetComponent<Rigidbody>().velocity = Vector3.zero;
		}


		float horizontalAxisInput = Input.GetAxis ("Horizontal");

		if (Mathf.Abs (horizontalAxisInput) > 0.01f) { //for any hardware calibration errors

			//Turn( horizontalAxisInput*RotationSpeed*(Time.deltaTime) ); 
			GetComponent<Rigidbody> ().angularVelocity = Vector3.up * horizontalAxisInput * RotationSpeed;
			//Debug.Log("horizontal axis ANG VEL = " + GetComponent<Rigidbody>().angularVelocity);
		}
		else {
			GetComponent<Rigidbody> ().angularVelocity = Vector3.zero * horizontalAxisInput * RotationSpeed;
		}

	}

	void Move( float amount ){
		transform.position += transform.forward * amount;
	}
	
	void Turn( float amount ){
		transform.RotateAround (transform.position, Vector3.up, amount );
	}

	void OnCollisionEnter(Collision collision){ //happens before the update loop and before the coroutine loop
		collisionObject = collision.gameObject;
		Debug.Log (collision.gameObject.name);
	}

	//only in x & z coordinates
	public void SetRandomLocation(){
		//based on the wall bounds, pick a location

		/*float wallBuffer = exp.config.bufferBetweenObjectsAndWall;

		float randomXPos = Random.Range(exp.environmentController.WallsXPos.position.x - wallBuffer, exp.environmentController.WallsXNeg.position.x + wallBuffer);
		float randomZPos = Random.Range(exp.environmentController.WallsZPos.position.z - wallBuffer, exp.environmentController.WallsZNeg.position.z + wallBuffer);

		Vector3 newPosition = new Vector3 (randomXPos, transform.position.y, randomZPos);


		if(randomXPos > exp.environmentController.WallsXPos.position.x || randomXPos < exp.environmentController.WallsXNeg.position.x){
			Debug.Log("avatar out of bounds in x!");
		}
		else if(randomZPos > exp.environmentController.WallsZPos.position.z || randomZPos < exp.environmentController.WallsZNeg.position.z){
			Debug.Log("avatar out of bounds in z!");
		}




		transform.position = newPosition;*/
	}

	//only in y axis
	public void SetRandomRotation(){
		//float randomYRotation = Random.Range(exp.config.minDegreeBetweenLearningTrials, exp.config.maxDegreeBetweenLearningTrials);
		//transform.RotateAround(transform.position, Vector3.up, randomYRotation);
	}
	
	//make avatar face the center of the environment
	public void RotateToEnvCenter(){
		//Vector3 center = exp.environmentController.center;
		//center = new Vector3(center.x, transform.position.y, center.z); //set the y coordinate to the avatar's -- should still look straight ahead!

		//transform.LookAt(center);
	}

}
