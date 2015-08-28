using UnityEngine;
using System.Collections;

public class LogController : MonoBehaviour {
	public string sName;
	private string logfile;
	private string eyeLogfile;
	[HideInInspector] public Logger_Threading log;

	public Logger_Threading eyeLog;
	private string eyeLogfile;

	private static LogController _instance;
	public static LogController Instance{
		get{
			return _instance;
		}
	}

	//When the program starts, log needs to be a singleton
	void Awake(){
		if (_instance != null) {
			Debug.Log ("Instance already exists!");
			return;
		}
		_instance = this;
<<<<<<< HEAD
		logfile = "DataFile/" + sName + "Log.txt"; //NOTE: THIS FOLDER MUST EXIST
		eyeLogfile = "DataFile/" + sName + "EyeLog.txt";
		//log.fileName = logfile;
		//log= GetComponent<Logger_Threading> ();
		//Logger_Threading.fileName = logfile;
		log.fileName = logfile;
		eyeLog.fileName = eyeLogfile;
=======

		logfile = "DataFile/" + sName + "Log.txt"; //NOTE: THIS FOLDER MUST EXIST
		eyeLogfile = "DataFile/" + sName + "EyeLog.txt";
		log = new Logger_Threading (logfile);
		eyeLog = new Logger_Threading (eyeLogfile);
		//log.fileName = logfile;
		//eyeLog.fileName = eyeLogfile;
		//log.fileName = logfile;
		//log= GetComponent<Logger_Threading> ();
		//Logger_Threading.fileName = logfile;

>>>>>>> e474c2063463b374265da3391ee224f5c493d5a0
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnExit(){
		log.close ();
		eyeLog.close ();
	}

	void OnApplicationQuit(){
		log.close ();
		eyeLog.close ();
	}
}
