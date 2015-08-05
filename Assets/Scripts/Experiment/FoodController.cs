using UnityEngine;
using System.Collections.Generic;


public class FoodController : MonoBehaviour {
	public GameObject spawnBanana;
	public GameObject spawnCherry;
	public GameObject player;
	public int numBananas;
	public int numCherries;
	public float distThresh;
	
	private List<GameObject> exists; //food and the player that exist in the world
	
	void Start(){
		exists = new List<GameObject> ();
		exists.Add(player);
	}
	
	void Update()
	{
		GameObject [] left = GameObject.FindGameObjectsWithTag ("Food");
		if (left.Length <=0) {
			SpawnBananas();
			SpawnCherries();
		}
	}

	void SpawnBananas(){
		SpawnSet (spawnBanana, numBananas);
	}

	void SpawnCherries(){
		SpawnSet (spawnCherry, numCherries);
	}

	void SpawnSet(GameObject foodToSpawn, int numFruit){
		emptyList();
		for (int i = 0; i<numFruit; i++) {
			SpawnObject(foodToSpawn, i);
		}
	}
	
	void SpawnObject(GameObject foodToSpawn, int ID){
		bool keepThis;
		bool itfits = false;
		float x;
		float z;
		int maxIt = 100000;
		int it = 0;

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
			if (!keepThis){
				it = 0;
				distThresh /=2;
				print (distThresh);
			}else{
				itfits = true;
			}
		} while(itfits == false);
		GameObject spawnedObj  = Instantiate(foodToSpawn, new Vector3(x,.5f,z), foodToSpawn.transform.rotation) as GameObject;
		exists.Add (spawnedObj);

		float randomRotation = Random.Range(0.0f, 360.0f);
		spawnedObj.transform.RotateAround(spawnedObj.transform.position, Vector3.up, randomRotation);

		spawnedObj.GetComponent<Food>().SetIDNum(ID);
	}
	
	float Distance(float x1, float z1, float x2, float z2){
		return Mathf.Sqrt(Mathf.Pow (x1 - x2,2)+Mathf.Pow (z1-z2,2));
	}
	
	void emptyList(){
		exists.Clear ();
		exists.Add (player);
	}

		
}
