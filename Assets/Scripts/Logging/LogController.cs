using UnityEngine;
using System.Collections;

public class LogController : MonoBehaviour {
	public string sName;
	private string logfile;
	private string logXfile;
	[HideInInspector] public Logger_Threading log;
	[HideInInspector] public Logger_Threading logX;

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
		logXfile = "DataFile/" + sName + "LogX.txt";
		//log.fileName = logfile;
		log= GetComponent<Logger_Threading> ();
		Logger_Threading.fileName = logfile;
		logX = GetComponent<Logger_Threading> ();
		Logger_Threading.fileName = logXfile;

		//log = new Logger_Threading (logfile);
		//logX = new Logger_Threading (logXfile);

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
