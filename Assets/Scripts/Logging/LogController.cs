using UnityEngine;
using System.Collections;

public class LogController : MonoBehaviour {
	public string sName;
	private string logfile;
	private string eyeLogfile;
	public Logger_Threading experimentLog;
	public LogEyeData eyeLog;
	private static LogController _instance;

	//When the program starts, log needs to be a singleton
	void Awake(){
		if (_instance != null) {
			Debug.Log ("Instance already exists!");
			return;
		}
		_instance = this;

		logfile = "DataFile/" + sName + "Log.txt"; //NOTE: THIS FOLDER MUST EXIST
        experimentLog.fileName = logfile;

        eyeLogfile = "DataFile/" + sName + "EyeLog.txt";
		eyeLog.fileName = eyeLogfile;
        eyeLog.StartLogging(eyeLogfile);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static LogController Instance{
		get{
			return _instance;
		}
	}

	public void OnExit(){
		experimentLog.close ();
		eyeLog.close ();
	}

	void OnApplicationQuit(){
		experimentLog.close ();
		eyeLog.close ();
	}
}
