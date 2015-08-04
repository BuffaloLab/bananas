using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class GiveReward : MonoBehaviour {
	[DllImport ("ASimplePlugin")]
	private static extern int PrintANumber();

	[DllImport ("ASimplePlugin")]
	private static extern int reward(bool on);
	
	// Use this for initialization
	void Start () {
		Debug.Log(PrintANumber());
		Debug.Log (reward (true));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
