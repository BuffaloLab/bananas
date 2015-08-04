using UnityEngine;
using System.Collections;

public class FoodLogger : MonoBehaviour {
	Logger_Threading experimentLog {get {return LogController.Instance.log; }}

	Food myFood;

	// Use this for initialization
	void Start () {
		myFood = GetComponent<Food> ();
		myFood.Initialize ();
		LogSpawned ();
	}

	void LogSpawned(){
		experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, " " + myFood.GetName() + " Spawned " + gameObject.transform.position + " ID " + myFood.IDnum); //add alpha value
	}

	void LogDestroyed(){
		experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, " " + myFood.GetName() + " Destroyed " + gameObject.transform.position + " ID " + myFood.IDnum); //add alpha value
	}
	
	void OnDestroy(){
		LogDestroyed ();
	}
}
