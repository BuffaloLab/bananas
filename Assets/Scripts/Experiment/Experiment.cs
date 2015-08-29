using UnityEngine;
using System.Collections;

//EXTEND OTHER EXPERIMENT SUBCLASSES FROM HERE FOR EASE OF USE IN REPLAY/ETC.
public abstract class Experiment : MonoBehaviour {
	public int ReplayFramesPerSecond = 60;
	public bool isReplay;
	public bool isSavingToPng;
	public bool drawCrosshair;
	public string PNGRecordingPath;
	public FoodController myFoodController;
	private static Experiment _instance;

	public void Awake(){
		if (_instance != null) {
			Debug.Log ("Instance already exists!");
			return;
		}
		_instance = this;
	}

	public static Experiment Instance{
		get{
			return _instance;
		}
	}
}
