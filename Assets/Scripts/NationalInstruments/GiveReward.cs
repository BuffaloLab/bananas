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

	// this defines the callback (delegate) that we will be
	// calling from the C code
	// something not right nere. have tried both int and intptr for taskhandle
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void eog_callback_del(IntPtr taskHandle,
	                                      Int32 everyNSamplesEventType,
	                                      UInt32 nSamples,
	                                      IntPtr callbackData);
	//public delegate void set_callback_del(IntPtr callback);

	[DllImport ("NidaqPlugin")]
	private static extern int eog_set_callback (
	[MarshalAs(UnmanagedType.FunctionPtr)]eog_callback_del 
			eye_data);
	
	[DllImport ("NidaqPlugin")]
	private static extern Int32 eog_start_task();
	
	[DllImport ("NidaqPlugin")]
	private static extern int eog_stop_task();

	// have tried interleaved (DAQmx_Val_GroupByScanNumber) as both IntPtr and Int32,
	// considered bool32 in C code, whatever the hell that is suppose to be. Okay, bool32
	// is zero or one, like bool, but is 32bits. bool is 8 bits, so I think making this 
	// int32 is the way to go. both interleaved and reserved are bool32, however reserved
	// requires an input of null, and I can only do this if I make it int?, as far as I can tell
	[DllImport ("NidaqPlugin")]
	private static extern float DAQmxReadAnalogF64 (
		IntPtr taskHandle,
		Int32 sampsPerChan,
		float timeout,
		Int32 interleaved,
		float[] data,
		UInt32 sizeArray,
		ref Int32 read,
		int? reserved);

	public Int32 numSamples = 1;
	public Int32 DAQmx_Val_GroupByChannel;
	public Int32 read;
	// in c++ this is data[1] seems most likely this one is the problem.
	// what the hell should this be?!?!?
	// try using float data; in c++ code in example callback and see if it 
	// barfs in not array.
	public float[] data;

	// Use this for initialization
	void Start () 
	{
		// this defines the callback that is called from the C++
		// code. 
		eog_callback_del set_callback =
			(taskHandle, everyNSamplesEventType, nSamples, callbackData) =>
			{	
			Debug.Log ("made callback");
			Debug.Log (taskHandle);
			Debug.Log (everyNSamplesEventType);
			Debug.Log (nSamples);
			Debug.Log (callbackData);
			Debug.Log (numSamples);
			Debug.Log (DAQmx_Val_GroupByChannel);
			Debug.Log (data);
			Debug.Log (read);
			Debug.Log (DAQmxReadAnalogF64(taskHandle,
			                              numSamples,
			                              10.0f,
			                              DAQmx_Val_GroupByChannel,
			                              data,
			                              1,
			                              ref read,
			                              null));
			Debug.Log ("read data");
			};
		Debug.Log ("start task");
		Debug.Log (DAQmx_Val_GroupByChannel = eog_start_task ());
		Debug.Log ("set callback");
		Debug.Log (eog_set_callback (set_callback));
		Debug.Log ("callback set");
		//Debug.Log(PrintANumber());
		// Debug.Log (reward (true));
		//Debug.Log (reward (0));
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Z)) {
			Debug.Log ("reward on");
			Debug.Log (reward (1));
		} else if (Input.GetKeyDown (KeyCode.X)) {
			Debug.Log ("stop reward");
			Debug.Log (reward (0));
		}
		else if(Input.GetKeyDown(KeyCode.Q)){
				Debug.Log ("stop task");
				Debug.Log (eog_stop_task());
		}
	}
}