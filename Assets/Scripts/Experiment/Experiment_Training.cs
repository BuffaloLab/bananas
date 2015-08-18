using UnityEngine;
using System.Collections;

public class Experiment_Training : Experiment {
	public GameObject player;
	public TrainingState state;
	public GameObject fruit;

	private int trialCount;

	private bool isRight;
	int level = 0;

	float distToBanana = 5.0f; 
	float angleToBanana = 10; //degrees
	

	// Use this for initialization
	void Start () {
		//state = GameObject.FindGameObjectWithTag ("State").GetComponent<TrainingState>;
		StartTrial ();
		player.GetComponent<AvatarControls_Training>().OnFoodCollisionDelegate += StartTrial;
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
		//Check to see if fruit is lined upl
		if (Physics.Raycast (player.transform.position, player.transform.forward)) {
			print ("lined up!");
		} else {
			print ("not lined up");
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
		if (level == 0) {
			SpawnFruitInFront(5f);
			SetDirection(20);
		} else if (level == 1) {
			//Overshoot now allowed. speed is slow after contact
		} else if (level == 2) {
			//Overshoot is now allowed. Not slow down.
		} else if (level == 3) {
			//Forward
			state.Move = TrainingState.Movement.forward;
			SpawnFruitInFront(5f);
		} else if (level == 4) {
			//Bring to center, then forward. 
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
}
