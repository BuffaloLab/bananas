using UnityEngine;
using System.Collections;

public class ExperimentForaging : Experiment {

	public MonkeyConfig ChosenMonkey_Foraging;

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