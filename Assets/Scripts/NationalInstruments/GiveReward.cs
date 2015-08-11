using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;

public class GiveReward : MonoBehaviour {
	
	[DllImport ("NidaqPlugin")]
	private static extern int reward(int on);

	//static int reward (int on) {
	//	return on;
	//}

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void set_callback_del(IntPtr callback);

	[DllImport ("NidaqPlugin")]
	private static extern int eog_set_callback (
	[MarshalAs(UnmanagedType.FunctionPtr)]set_callback_del 
			eye_data);
	
	[DllImport ("NidaqPlugin")]
	private static extern int eog_start_task();
	
	[DllImport ("NidaqPlugin")]
	private static extern int eog_stop_task();

	// Use this for initialization
	void Start () 
	{
		set_callback_del callback =
			(data) =>
				{	
				Debug.Log ("made callback");
				Debug.Log (data);
				};
		Debug.Log ("start task");
		Debug.Log (eog_start_task ());	
		Debug.Log ("set callback");
		Debug.Log (eog_set_callback (callback));
		Debug.Log ("callback set");
		//Debug.Log(PrintANumber());
		// Debug.Log (reward (true));
		//Debug.Log (reward (0));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Z)){
			Debug.Log ("reward on");
			Debug.Log (reward (1));
		}
		else if(Input.GetKeyDown(KeyCode.X)){
			Debug.Log ("stop reward");
			Debug.Log (reward (0));
			Debug.Log ("stop task");
			Debug.Log (eog_stop_task());
		}
	}
}