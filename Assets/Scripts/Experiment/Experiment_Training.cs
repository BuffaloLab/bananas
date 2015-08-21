using UnityEngine;
using System.Collections;

public class Experiment_Training : Experiment {
	public GameObject player;
	public TrainingState state;
	public GameObject fruit;

	GiveReward reward;
	private int trialCount;

	private bool isRight;
	public int level = 0;

	//build layerMask for line up steps. "Food" is layer 8
	int layerMask = 1 << 8;
	RaycastHit hit;

	float distToBanana = 8.0f; 
	bool inline;

	float angleRange = 20;
	bool isManual = false;


	// Use this for initialization
	void Start () {

		//state = GameObject.FindGameObjectWithTag ("State").GetComponent<TrainingState>;
		player.GetComponent<AvatarControls_Training>().OnFoodCollisionDelegate += StartTrial;
		player.GetComponent<AvatarControls_Training>().OnFoodLineupDelegate += SetInLine;
		reward = GameObject.FindGameObjectWithTag ("Reward").GetComponent<GiveReward> ();
		drawCrosshair = true;
		StartTrial ();
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
			if (isManual){
				isManual=false;
			}else{
				isManual=true;
			}
			print ("isManual is now "+ isManual);
		}
		if (Input.GetKeyDown (KeyCode.Equals)) {
			angleRange++;
			print ("angleRange = "+angleRange);
		}
		if (Input.GetKeyDown (KeyCode.Minus)) {
			angleRange--;
			print ("angleRange = "+angleRange);

		}

	//Check to see if fruit is lined upl
	/*	if (Physics.Raycast (player.transform.position, player.transform.forward, out hit, Mathf.Infinity,layerMask)) {
			drawCrosshair = false;
			inline = true;
		} else {
			drawCrosshair = true;
			inline = false;
		}*/

		switch (level) {
		case(0):
			//Bring to center, one direction.
			if (inline){
				reward.RewardAndFreeze(3);
				StartCoroutine(DestroyFoodAfterFreeze());
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
		inline = false;
	}
	

	void ResetPlayer(){
		ResetPlayer (0f);
	}

	void ResetPlayer(float angle){
		player.transform.position = new Vector3(0f, .5f, 0f);
		player.GetComponent<Rigidbody> ().angularVelocity = Vector3.zero;
		player.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		//Work out transform for rotation of player on start
		player.transform.RotateAround (Vector3.zero, Vector3.up, angle);

	}

	void StartTrial(){
		StartCoroutine (StartTrialAfterFreeze());
	}

	IEnumerator StartTrialAfterFreeze(){
		while (reward.isFrozen) {
			yield return new WaitForSeconds(.01f);
		}
		ResetPlayer ();
		drawCrosshair = true;
		inline = false;
		switch(level) {
		case (0):
			SpawnFruitInFront(distToBanana);
			if (isManual){
				SetDirection(angleRange);
			} else {
				SetDirection(Random.Range(0,angleRange));
			}
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
		transform.position = Vector3.zero;
		myFoodController.SpawnObjectAt (fruit,transform.forward * dist+ new Vector3(0f,.5f,0f), trialCount);
	}

	IEnumerator DestroyFoodAfterFreeze(){
		GameObject food = GameObject.FindGameObjectWithTag ("Food");
		while (reward.isFrozen) {
			yield return new WaitForSeconds(.01f);
		}
		Destroy (food);
	}

	void SetInLine(){
		inline = true;
		drawCrosshair = false;
	}

}
