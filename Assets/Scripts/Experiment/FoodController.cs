using UnityEngine;
using System.Collections.Generic;


public class FoodController : MonoBehaviour {
	Experiment exp;

	public GameObject spawnBanana;
	public GameObject spawnCherry;
	public GameObject player;
	public int numBananas;
	public int numCherries;
	public float distThresh;
	
	private List<GameObject> exists; //food and the player that exist in the world
	
	void Start(){
		exp = GameObject.FindGameObjectWithTag ("Experiment").GetComponent<Experiment> ();
		exists = new List<GameObject> ();
		exists.Add(player);
	}

	public void SpawnBananas(){
		SpawnSet (spawnBanana, numBananas);
	}

	public void SpawnCherries(){
		SpawnSet (spawnCherry, numCherries);
	}

	public void SpawnSet(GameObject foodToSpawn, int numFruit){
		emptyList();
		for (int i = 0; i<numFruit; i++) {
			SpawnObject(foodToSpawn, i);
		}
	}

	public void SpawnObjectAt(GameObject foodToSpawn, Vector3 location, int nameID){
		GameObject spawnedObj  = Instantiate(foodToSpawn, location, foodToSpawn.transform.rotation) as GameObject;
		float randomRotation = Random.Range(0.0f, 360.0f);
		spawnedObj.transform.RotateAround(spawnedObj.transform.position, Vector3.up, randomRotation);
		spawnedObj.GetComponent<Food>().SetNameID(nameID);
	}

	public GameObject SpawnObjectByName(string foodName, int nameID){ //for use in replay, in particular
		GameObject spawned = null;

		if (foodName == spawnBanana.GetComponent<Food> ().GetName ()) {
			spawned = SpawnObject (spawnBanana, nameID);
		} else if (foodName == spawnCherry.GetComponent<Food> ().GetName ()) {
			spawned = SpawnObject (spawnCherry, nameID);
		} else {
			Debug.Log ("Cannot spawn food of name: " + foodName);
		}

		return spawned;
	}

	
	public GameObject SpawnObject(GameObject foodToSpawn, int nameID){

		bool keepThis;
		bool itfits = false;
		float x = 0;
		float z = 0;
		int maxIt = 100000;
		int it = 0;

		if (!exp.isReplay) {
			do {
				do {
					keepThis = true;
					x = Random.Range (-9f, 9f);
					z = Random.Range (-9f, 9f);
					for (int i = 0; i<exists.Count; i++) {
						if (Distance (exists [i].transform.position.x, exists [i].transform.position.z, x, z) < distThresh) {
							keepThis = false;
						}
					}
					it++;
				} while(keepThis == false && it<maxIt);
				if (!keepThis) {
					it = 0;
					distThresh /= 2;
					print (distThresh);
				} else {
					itfits = true;
				}
			} while(itfits == false);
		}
		GameObject spawnedObj = Instantiate (foodToSpawn, new Vector3 (x, .5f, z), foodToSpawn.transform.rotation) as GameObject;
		exists.Add (spawnedObj);

		float randomRotation = Random.Range (0.0f, 360.0f);
		spawnedObj.transform.RotateAround (spawnedObj.transform.position, Vector3.up, randomRotation);

		spawnedObj.GetComponent<Food> ().SetNameID (nameID);

		return spawnedObj;

	}
	
	float Distance(float x1, float z1, float x2, float z2){
		return Mathf.Sqrt(Mathf.Pow (x1 - x2,2)+Mathf.Pow (z1-z2,2));
	}
	
	void emptyList(){
		exists.Clear ();
		exists.Add (player);
	}
	
}
