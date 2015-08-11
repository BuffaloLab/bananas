using UnityEngine;
using System.Collections;

public class ExperimentForaging : Experiment {

	public MonkeyConfig ChosenMonkey_Foraging;
	public GameObject player;
	int remaining = 0; //This will need to be set from the config file, once we get there

	// Use this for initialization
	void Start () {
		player.GetComponent<AvatarControls>().OnFoodCollisionDelegate += removeOne;
	}
	
	// Update is called once per frame
	void Update () {
		if (!isReplay && remaining<=0) {
			myFoodController.SpawnBananas ();
			myFoodController.SpawnCherries ();
			GameObject[] toCount = GameObject.FindGameObjectsWithTag("Food");
			remaining = toCount.Length;
		}
		print (remaining);
	}

	void removeOne(){
		remaining--;
	}
}