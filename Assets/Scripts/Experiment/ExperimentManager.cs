using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExperimentManager : Experiment {
	public GameObject player;
	public int numEncoding;
	public int numDistract;
	public GameObject distractFruit;//Distractor fruit-->usually cherries
	public GameObject targetFruit;//Fruit to find--> usually Bananas
	public float distThresh;
	public float bubble;
	
	[HideInInspector]public List<GameObject> trialFruit;
	private int thisFruit;
	private float height = 0.5f;

	private int encodingNow;
	private int distractorNow;


	public static ExperimentManager _instance;
	public static ExperimentManager Instance{
		get{
			return _instance;
			}
	 }

	public enum stateIs{
		target,
		distractor,
		recall,
		cleanup
	}
	public stateIs state;

	void Awake(){
		if (_instance != null) {
			Debug.Log ("Instance already exists!");
			return;
		}
		_instance = this;
	}

	void Start(){
		//initiate the list of fruit 
		trialFruit = new List<GameObject>();
		new_Trial (player.transform.position.x, player.transform.position.z);
	}

	void Update(){
		if (state == stateIs.recall) {
			float dist = Distance(
				trialFruit[0].transform.position.x,trialFruit[0].transform.position.z,
				player.transform.position.x,player.transform.position.z);
			if (dist<bubble){
				float new_x = player.transform.position.x+Mathf.Sin (player.transform.rotation.x);
				float new_z = player.transform.position.z+Mathf.Cos (player.transform.rotation.z);
				GameObject spawnedObj  = Instantiate(targetFruit, new Vector3(new_x,.5f,new_z),Quaternion.identity) as GameObject;
				state = stateIs.cleanup;
				//new_Trial (player.transform.position.x,player.transform.position.z);
			}
		}
	}

	public void nextFruit(){
		//If the last trial was a target, this one is a distractor
		if (state == stateIs.target) {
			state = stateIs.distractor;
			print("State is now distractor");
		}else if (state == stateIs.distractor && (distractorNow >= numDistract)) {
			distractorNow = 0;
			encodingNow++;
			if (encodingNow < numEncoding) {
				trialFruit [0].SetActive (true);
				state = stateIs.target;
				print("State is now target");
			} else {//Recall Step...trip switch.
				state = stateIs.recall;
				print("State is now recall");
			}

		}

		if (state == stateIs.distractor && distractorNow < numDistract) {
			//trialFruit[thisFruit-1].SetActive(false);
			distractorNow++;//Advance to next distractor
			thisFruit++;
			trialFruit [thisFruit].SetActive (true);
			/*
			if (state == stateIs.target) {
				//Still Encoding
				if (encodingNow < numEncoding) {
					trialFruit [0].SetActive (true);
				} else {//Recall Step...trip switch.
					state = stateIs.recall;
					print("State is now recall");
				}
			}*/
		}		
	}

	public void start_Trial(){
		thisFruit = 0;
		trialFruit [thisFruit].SetActive (true);
		distractorNow = 0;
		encodingNow = 0;
		state = stateIs.target;
	}

	public void new_Trial(float last_x, float last_z){
		trialFruit.Clear ();
		bool farEnough = false;
		float x; 
		float z;
		do {
			x = Random.Range (-9f, 9f);
			z = Random.Range (-9f, 9f);
			if (Distance (last_x,last_z,x,z)>distThresh){
				farEnough = true;
			}
		} while(farEnough==false);

		GameObject spawnedObj  = Instantiate(targetFruit, new Vector3(x,.5f,z),Quaternion.identity) as GameObject;
		trialFruit.Add (spawnedObj);
		trialFruit[0].SetActive(false);

		for (int i = 0; i<numEncoding; i++) {
			for (int j = 0; j<numDistract;j++){
				farEnough = false;
				float fruit_x = Random.Range (-9f, 9f);
				float fruit_z = Random.Range (-9f, 9f);
				while(farEnough == false) {
					fruit_x = Random.Range (-9f, 9f);
					fruit_z = Random.Range (-9f, 9f);
					if (((Distance (fruit_x,fruit_z,trialFruit[trialFruit.Count-1].transform.position.x,trialFruit[trialFruit.Count-1].transform.position.z))>distThresh)||(Distance(fruit_x,fruit_z,x,z)>distThresh)){
						farEnough = true;
					}
				}
				spawnedObj  = Instantiate(distractFruit, new Vector3(fruit_x,.5f,fruit_z),Quaternion.identity) as GameObject;
				trialFruit.Add (spawnedObj);
				trialFruit[trialFruit.Count-1].SetActive(false);
			}
		}
		start_Trial ();
	}

	float Distance(float x1, float z1, float x2, float z2){
		return Mathf.Sqrt(Mathf.Pow (x1 - x2,2)+Mathf.Pow (z1-z2,2));
	}

	
}
