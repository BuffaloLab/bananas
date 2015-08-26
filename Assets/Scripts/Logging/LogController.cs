using UnityEngine;
using System.Collections;

public class LogController : MonoBehaviour {
	public string sName;
	private string logfile;
	[HideInInspector] public Logger_Threading log;
	//public Logger_Threading eyeLog;

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
		logfile = "DataFile/"+sName+"Log.txt"; //NOTE: THIS FOLDER MUST EXIST

		//log.fileName = logfile;
		log= GetComponent<Logger_Threading> ();
		Logger_Threading.fileName = logfile;

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnExit(){
		log.close ();
	}

	void OnApplicationQuit(){
		log.close ();
	}
}
