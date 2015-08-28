using UnityEngine;
using System.Collections;

public abstract class LogStuff : MonoBehaviour {
	//public Experiment exp {get {return Experiment.Instance;}}
	public Logger_Threading experimentLog {get {return LogController.Instance.log;}}
	public Logger_Threading eyeLog {get{return LogController.Instance.eyeLog;}}
	//Corey has a "LogTextSeparator" here...beware.
}
