using UnityEngine;
using System.Collections;

public class ObjectLogger : MotherOfLogs {
	//Experiment exp;
	//public Experiment exp {get {return Experiment.Instance;}}
	//public Logger_Threading experimentLog {get {return LogController.Instance.log;}}
	//public Logger_Threading eyeLog {get{return LogController.Instance.eyeLog;}}

	void Awake(){
		//exp = GameObject.FindGameObjectWithTag ("Experiment").GetComponent<Experiment>();
	}

	// Use this for initialization
	void Start () {

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
		eyeLog.Log (GameClock.Instance.SystemTime_Milliseconds, 
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