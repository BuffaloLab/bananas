using UnityEngine;
using System.Collections;

public class FoodLogger : MonoBehaviour {
	Logger_Threading experimentLog {get {return LogController.Instance.log; }}
	Experiment exp;

	Food myFood;

	// Use this for initialization
	void Start () {
		exp = GameObject.FindGameObjectWithTag ("Experiment").GetComponent<Experiment>();
		myFood = GetComponent<Food> ();
		LogSpawned ();
		LogPosition ();
		LogRotation ();
	}

	void LogSpawned(){
		if (!exp.isReplay) {
			experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, myFood.GetName () + ",SPAWNED"); //add alpha value
		}
	}

	void LogAlpha(){
		if (!exp.isReplay) {
			experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, myFood.GetName () + ",ALPHA," + myFood.GetAlpha () + ",ID,"); //add alpha value
		}
	}

	void LogDestroyed(){
		if (!exp.isReplay) {
			experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, myFood.GetName () + ",DESTROYED," + 
				gameObject.transform.position.x + 
				"," + gameObject.transform.position.y + 
				"," + gameObject.transform.position.z); //add alpha value
		}
	}
	
	void OnDestroy(){
		LogDestroyed ();
	}

	void LogPosition(){
		if (!exp.isReplay) {
			experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, myFood.GetName () + ",POSITION," +
			                   gameObject.transform.position.x + 
			                   "," + gameObject.transform.position.y + 
			                   "," + gameObject.transform.position.z); //add alpha value
		}
	}
	void LogRotation(){
		if (!exp.isReplay) {
			experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, myFood.GetName () + ",ROTATION," +
			                   gameObject.transform.rotation.eulerAngles.x + 
			                   "," + gameObject.transform.rotation.eulerAngles.y + 
			                   "," + gameObject.transform.rotation.eulerAngles.z); //add alpha value
		}
	}
}