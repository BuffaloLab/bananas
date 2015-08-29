using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class ExperimentRecall : Experiment {
	//Get the ability to log stuff!
	Logger_Threading experimentLog {get {return LogController.Instance.experimentLog; }}
	Logger_Threading eyeLog {get{return LogController.Instance.eyeLog;}}

	Stopwatch recallTimer;
	long maxRecallTimeMS = 60000; //in milliseconds

	//Subject config settings
	public MonkeyConfig ChosenMonkey_Foraging;

	// Link to the rest of the game
	public GameObject player;
	public GameObject distractor;
	public GameObject target;

	//Some parameters
	//THESE SHOULD BE MOVED TO MONKEYCONFIG AT SOME POINT!
	public int numEncodingTrials; //num cycles of target-distractor-distractor before recall happens
	public int numDistractorFruit; //num distractors per cycle
	public float distanceThreshold;
	public float bubbleRadius;

	Vector3 bananaPosition;

	int currentEncoding; //current cycle
	int currentDistractor;
	int trialCount; //number of total trial units

	public enum State{
		target,
		distractor,
		recall,
		cleanup
	}

	public State stateIs ;

	// Use this for initialization
	void Start () {
		recallTimer = new Stopwatch ();

		//The following subscribes the NextFruit() method to the OnFoodCollision Delegate.
		//Thus, whenever the OnFoodCollision event happens, NextFruit() will be called.
		player.GetComponent<AvatarControls_MainTask>().OnFoodCollisionDelegate += NextFruit;
		NewTrial ();
	}

	// Update is called once per frame
	void Update () {
		if (!isReplay) {
			//Check to see if there are any fruit left
			if (stateIs==State.recall) {//if this is a recall trial
				if (Distance(player.transform.position.x,player.transform.position.z,bananaPosition.x,bananaPosition.z)<bubbleRadius){
					//Bubble collision
					NextFruit();
				}
				if(recallTimer.ElapsedMilliseconds > maxRecallTimeMS){
					UnityEngine.Debug.Log("Recall took too long.");
					recallTimer.Stop();
					recallTimer.Reset();
					NewTrial();
				}
			}
		}
	}

	void NewTrial(){
		//experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, ",Start Trail," + trialCount);
		stateIs = State.target;
		print ("State is now Target");
		//Start over counting
		currentEncoding = 0;
		currentDistractor = 0;
		//Set banana position for this trial
		//Find a good location for the distractor
		float x;
		float z;
		do {
			x = RandomPos ();
			z = RandomPos ();
		} while(Distance (x,z,player.transform.position.x,player.transform.position.z)<distanceThreshold);
		bananaPosition =new Vector3(x,target.transform.position.y,z);
		//
		SpawnTarget();
	}

	//Change state, advance to next fruit. 
	void NextFruit(){
		UnityEngine.Debug.Log ("Next fruit!");
		if (!isReplay) { //TODO: TRY TO MAKE THIS CHECK LESS. MAKE THIS LESS HACK-Y.
			switch (stateIs) {
			case State.target:
				stateIs = State.distractor;
				SpawnDistractor ();
				print ("State is now Distractor");
				break;
			case State.distractor:
				if (currentDistractor >= numDistractorFruit) {//if no more distractors are needed
					currentDistractor = 0;
					currentEncoding++;
					if (currentEncoding < numEncodingTrials) {//if more encoding trials are needed
						//move to target step;
						stateIs = State.target;
						SpawnTarget ();
						print ("State is now Target");
					} else {//time for recall step
						//move to recall step
						stateIs = State.recall;
						recallTimer.Start ();
						print ("State is now Recall");
					}
				} else {//more distractor fruit are needed
					SpawnDistractor ();
				}
				break;
			case State.recall:
				stateIs = State.cleanup;
					//Spawn a fruit in front of the player;
				Vector3 newPos = player.transform.position + player.transform.forward * 1;					
				if ((newPos.x > 9.5f) || (newPos.x < -9.5f) || newPos.x > 9.5f || newPos.x < -9.5f) { //new location is outside a wall! make game object in old spot
					GameObject lastTarget = Instantiate (target, bananaPosition, target.transform.rotation) as GameObject;
				} else { //new location is good! make a banana there!
					GameObject lastTarget = Instantiate (target, newPos, target.transform.rotation) as GameObject;
				}
				break;
			case State.cleanup:
					//start next trial;
				NewTrial ();
				break;
			}
		}
	}

	void SpawnTarget(){
		if (!isReplay) {
			myFoodController.SpawnObjectAt (target, bananaPosition, currentEncoding);
		}
	}

	void SpawnDistractor(){
		currentDistractor++;
		//Find a good location for the distractor
		if (!isReplay) {
			float x;
			float z;
			do {
				x = RandomPos ();
				z = RandomPos ();
			} while((Distance (x,z,player.transform.position.x,player.transform.position.z)<distanceThreshold) || (Distance (x,z,bananaPosition.x,bananaPosition.z)<distanceThreshold));
			//Spawn distractor;
			int distractorID = numDistractorFruit * currentEncoding + currentDistractor;//keeps track of distractor ID between target steps.
			myFoodController.SpawnObjectAt (distractor, new Vector3 (x, distractor.transform.position.y, z), distractorID);
		}
	}

	float Distance(float x1, float z1, float x2, float z2){
		return Mathf.Sqrt(Mathf.Pow (x1 - x2,2)+Mathf.Pow (z1-z2,2));
	}

	float RandomPos(){
		return Random.Range (-9f,9f);
	}
}