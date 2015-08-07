using UnityEngine;
using System.Collections;

public class ObjectLogger : MonoBehaviour {
	Logger_Threading experimentLog {get {return LogController.Instance.log; }}
	Experiment exp;

	// Use this for initialization
	void Start () {
		exp = GameObject.FindGameObjectWithTag ("Experiment").GetComponent<Experiment>();
	}
	
	// Update is called once per frame
	void Update () { 
		if (!exp.isReplay) {
			LogPosition ();
			LogRotation ();
		}
	}

	void LogPosition(){
		//experimentLog.Log("Test_position");
		experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, 
		                   gameObject.name + ",POSITION," + gameObject.transform.position.x + "," + gameObject.transform.position.y + "," + gameObject.transform.position.z);
	}

	void LogRotation(){
		//experimentLog.Log ("Test_rotation");
		experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, gameObject.name + ",ROTATION," + 		                   
		                   gameObject.transform.rotation.eulerAngles.x + 
		                   "," +gameObject.transform.rotation.eulerAngles.y + 
		                   "," +gameObject.transform.rotation.eulerAngles.z);
	}
}