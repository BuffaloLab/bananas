using UnityEngine;
using System.Collections;

public class ExperimentRecall : Experiment {

	//Subject config settings
	public MonkeyConfig ChosenMonkey_Foraging;

	// Link to the rest of the game
	public GameObject player;
	public GameObject distractor;
	public GameObject target;

	//Some parameters
	//THESE SHOULD BE MOVED TO MONKEYCONFIG AT SOME POINT!
	public int numEncodingTrials;
	public int numDistractorFruit;
	public float distanceThreshold;
	public float bubbleRadius;

	public enum State{
		target,
		distractor,
		recall,
		cleanup
	}

	public State stateIs ;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!isReplay) {
			GameObject [] left = GameObject.FindGameObjectsWithTag ("Food");
			if (left.Length <= 0) {
				myFoodController.SpawnBananas ();
				myFoodController.SpawnCherries ();
			}
		}
	}
}