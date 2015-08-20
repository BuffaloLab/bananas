using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;

public class GiveReward : MonoBehaviour {
	
	#if NIDAQ
		[DllImport ("NidaqPlugin")]
		private static extern int Reward(int on);
	#else
		int Reward(int on)
		{
			if (on == 1) {
			Debug.Log("no nidaq, start reward");
			} else {
			Debug.Log ("no nidaq, stop reward");
			}
		return on;
		}
	#endif

	// Use this for initialization
	void Start () 
	{

	}

	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Z)) {
			Debug.Log ("reward on");
			Debug.Log (Reward (1));
		} else if (Input.GetKeyDown (KeyCode.X)) {
			Debug.Log ("stop reward");
			Debug.Log (Reward (0));
		}
	}
}