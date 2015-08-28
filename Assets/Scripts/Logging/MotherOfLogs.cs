using UnityEngine;
using System.Collections;

public abstract class MotherOfLogs : MonoBehaviour {
	public Logger_Threading experimentLog {get {return LogController.Instance.log; }}
	public Logger_Threading eyeLog{get{return LogController.Instance.eyeLog;}}
	public Experiment exp {get {return Experiment.Instance;}}
}