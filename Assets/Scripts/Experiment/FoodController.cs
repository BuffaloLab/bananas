using UnityEngine;
using System.Collections;


public class FoodController : MonoBehaviour {

	public GameObject spawnBanana;
	public GameObject spawnCarrot;
	public GameObject spawnCherry;
	public GameObject player;
	public int numFruit;

	void Start(){
		SpawnFood (spawnBanana);
	}

	void Update()
	{
		GameObject [] left = GameObject.FindGameObjectsWithTag ("Food");
		if (left.Length <=0) {
			print ("None");
			SpawnFood(spawnBanana); //TODO: switch to other fruit when necessary
		}
	}

	void SpawnFood(GameObject foodToSpawn){
		foodToSpawn.SetActive(true);
		foodToSpawn.tag = "Food";
		for (int i = 0; i<numFruit; i++) {
			SpawnObject(foodToSpawn, i); //set ID number to i
		}
	}

	void SpawnObject(GameObject foodToSpawn, int ID){
		float x = Random.Range (-9f, 9f);
		float z = Random.Range (-9f, 9f);
		Instantiate(foodToSpawn, new Vector3(x,.5f,z), foodToSpawn.transform.rotation);

		float randomRotation = Random.Range (0.0f, 360.0f);
		foodToSpawn.transform.RotateAround (transform.position, Vector3.up, randomRotation);

		foodToSpawn.GetComponent<Food> ().SetIDNum (ID);
	}
		
}
