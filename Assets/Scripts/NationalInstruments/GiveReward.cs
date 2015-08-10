using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class GiveReward : MonoBehaviour {
	 
	[DllImport ("NidaqPlugin")]
	private static extern int reward(int on);

	static bool CheckLibrary(string fileName) {
		return reward (fileName) == IntPtr.Zero;
	}
	//[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	//public delegate void set_callback(IntPtr data);

	//[DllImport ("NidaqPlugin")]
	//private static extern int eog_set_callback (
	//	[MarshalAs(UnmanagedType.FunctionPtr)]set_callback 
	//		eog_callback);
	
	//[DllImport ("ASimplePlugin")]
	//private static extern int reward(int on);
	
	// Use this for initialization
	void Start () {
		//eog_set_callback (eye_data);
		//Debug.Log(PrintANumber());
		// Debug.Log (reward (true));
		//Debug.Log (reward (0));
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Z)){
			Debug.Log (reward (1));
		}
		else if(Input.GetKeyDown(KeyCode.X)){
			Debug.Log (reward (0));
		}
	}

	void eye_data (int data) {
		Debug.Log ("made callback");
		Debug.Log (data);
	}
}
