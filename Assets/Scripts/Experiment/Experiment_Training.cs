using UnityEngine;
using System.Collections;

public class Experiment_Training : Experiment {
	public GameObject player;
	public TrainingState state;
	public GameObject fruit;

	private int trialCount;

	private bool isRight;
	int level = 0;

	//build layerMask for line up steps. "Food" is layer 8
	int layerMask = 1 << 8;
	RaycastHit hit;

	float distToBanana = 5.0f; 
	float angleToBanana = 10; //degrees
	bool inline;

	float angleRange = 20;
	bool isManual = false;


	// Use this for initialization
	void Start () {
		//state = GameObject.FindGameObjectWithTag ("State").GetComponent<TrainingState>;
		StartTrial ();
		player.GetComponent<AvatarControls_Training>().OnFoodCollisionDelegate += StartTrial;
		drawCrosshair = true;
	}
	
	// Update is called once per frame
	void Update () {
		//Increase Level
		if (Input.GetKeyDown (KeyCode.Q)) {
			level++;
		}
		//Decrease Level
		if (Input.GetKeyDown(KeyCode.A)){
			level--;
		}
		//set left, right where applicable
		if (Input.GetKeyDown (KeyCode.L)) {
			isRight = false;
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			isRight = true;
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			print("You pressed the space bar!!");
		}

		if (Input.GetKeyDown (KeyCode.M)) {
			isManual ^= isManual;
			print ("isManual is now "+ isManual);
		}

		//Check to see if fruit is lined upl
		if (Physics.Raycast (player.transform.position, player.transform.forward, out hit, Mathf.Infinity,layerMask)) {
			drawCrosshair = false;
			inline = true;
		} else {
			drawCrosshair = true;
			inline = false;
		}

		switch (level) {
		case(0):
			//Bring to center, one direction.
			if (inline){
				DestroyFood ();
				StartTrial();
			}
			break;
		case(1):
			break;
		case(2):
			break;
		case(3):
			break;
		case(4):
			break;
		}
	}
	

	void ResetPlayer(){
		ResetPlayer (0f);
	}

	void ResetPlayer(float angle){
		player.transform.position = new Vector3(0f, .5f, 0f);
		//Work out transform for rotation of player on start
		player.transform.RotateAround (Vector3.zero, Vector3.up, angle);
	}

	void StartTrial(){
		ResetPlayer ();
		switch(level) {
		case (0):
			SpawnFruitInFront(5f);
			SetDirection(20);
			break;
		case(1):
			//Overshoot now allowed. speed is slow after contact
			break;
		case(2):
			//Overshoot is now allowed. Not slow down.
			break;
		case(3):
			//Forward
			state.Move = TrainingState.Movement.forward;
			SpawnFruitInFront(5f);
			break;
		case(4):
			//Bring to center, then forward.
			break;
		}
		trialCount++;
	}

	//Must be called AFTER the fruit is spawned, if fruit is being guided by player.transform.forward
	void SetDirection(float angle){
		if (isRight){
			state.Move = TrainingState.Movement.right;
			ResetPlayer (-angle);
		}else{
			state.Move = TrainingState.Movement.left;
			ResetPlayer (angle);
		}
	}

	void SpawnFruitInFront(float dist){
		myFoodController.SpawnObjectAt (fruit, player.transform.forward * dist+ new Vector3(0f,.5f,0f), trialCount);
	}

	void DestroyFood(){
		GameObject food = GameObject.FindGameObjectWithTag ("Food");
		Destroy (food);
	}
}
