using UnityEngine;
using System.Collections;

public class FoodLogger : MonoBehaviour {
	Logger_Threading experimentLog {get {return LogController.Instance.log; }}
	Experiment exp;

	Food myFood;

	void Awake(){
		exp = GameObject.FindGameObjectWithTag ("Experiment").GetComponent<Experiment> ();
		myFood = GetComponent<Food> ();
	}

	// Use this for initialization
	void Start () {
		LogSpawned ();
		LogPosition ();
		LogRotation ();
	}

	void LogSpawned(){
		if (!exp.isReplay) {
			experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, myFood.GetName () + ",SPAWNED");
		}
	}

	public void LogAlpha(float alpha){ //should be called when alpha gets set in Food.cs
		if (!exp.isReplay) {
			experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, gameObject.name + ",ALPHA," + alpha);
		}
	}
	
	void LogDestroyed(){
		//Destroy was getting called on an inactive object that had never had exp set. thus, check for null exp here.
		if (!exp.isReplay) {
			experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, myFood.GetName () + ",DESTROYED," + 
				gameObject.transform.position.x + 
				"," + gameObject.transform.position.y + 
				"," + gameObject.transform.position.z);
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
			                   "," + gameObject.transform.position.z);
		}
	}

	void LogRotation(){
		if (!exp.isReplay) {
			experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, myFood.GetName () + ",ROTATION," +
			                   gameObject.transform.rotation.eulerAngles.x + 
			                   "," + gameObject.transform.rotation.eulerAngles.y + 
			                   "," + gameObject.transform.rotation.eulerAngles.z);
		}
	}
}