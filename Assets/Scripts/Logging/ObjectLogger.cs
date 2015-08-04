using UnityEngine;
using System.Collections;

public class ObjectLogger : MonoBehaviour {
	Logger_Threading experimentLog {get {return LogController.Instance.log; }}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		LogPosition ();
		LogRotation ();
	}

	void LogPosition(){
		//experimentLog.Log("Test_position");
		experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, " " + gameObject.name + "Position" + gameObject.transform.position);
	}

	void LogRotation(){
		//experimentLog.Log ("Test_rotation");
		experimentLog.Log (GameClock.Instance.SystemTime_Milliseconds, " " + gameObject.name + "Rotation" + transform.rotation);
	}
}
