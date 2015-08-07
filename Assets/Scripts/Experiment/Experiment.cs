using UnityEngine;
using System.Collections;

//EXTEND OTHER EXPERIMENT SUBCLASSES FROM HERE FOR EASE OF USE IN REPLAY/ETC.
public abstract class Experiment : MonoBehaviour {
	public bool isReplay;
	public bool isSavingToPng;
	public string PNGRecordingPath;
	public FoodController myFoodController;
}
