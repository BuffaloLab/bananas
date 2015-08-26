using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;

public class GetData : MonoBehaviour 
{
	// this declares the callback (delegate) that we will be
	// calling from the C code
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void Eog_callback_del(IntPtr taskHandle,
	                                      Int32 everyNSamplesEventType,
	                                      UInt32 nSamples,
	                                      IntPtr callbackData);

	// so we can send a string to know which channel is which
	[DllImport("NidaqPlugin", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr eog_start_task(String channel);

	// import the c++ call to set the call back
	[DllImport ("NidaqPlugin")]
	private static extern int eog_set_callback (
		[MarshalAs(UnmanagedType.FunctionPtr)]Eog_callback_del 
		eye_data, IntPtr taskHandle);
	
	// [DllImport ("NidaqPlugin")]
	// private static extern Int32 eog_start_task();
	
	[DllImport ("NidaqPlugin")]
	private static extern Int32 eog_stop_task(IntPtr taskHandle);
	
	[DllImport ("NidaqPlugin")]
	private static extern Double eog_return_data(IntPtr taskHandle);
	
	// private Int32 numSamples = 1;
	private Int32 DAQmx_Val_GroupByChannel;
	private Int32 read;
	// set this to a ridiculous number so sure we are getting data
	public Double data = 20;
	private IntPtr taskHandle1;
	private IntPtr taskHandle2;
	public String channel1 = "Dev1/ai3";
	public String channel2 = "Dev1/ai4";

	// Use this for initialization
	void Start () 
	{
		// this defines the callback that is called from the C++
		// code. 
		Eog_callback_del eog_callback =
			(IntPtr taskHandle, Int32 everyNSamplesEventType, UInt32 nSamples, IntPtr callbackData) =>
		{	
			Debug.Log ("made callback");
			Debug.Log (data);
			try
			{
				Debug.Log (data = eog_return_data(taskHandle));
			}
			catch (Exception ex)
			{
				Debug.Log ("Exception");
				Debug.Log (ex.Message);
				Debug.Log (ex.GetBaseException());
			}
		};
		Debug.Log ("start task");
		Debug.Log (taskHandle1 = eog_start_task (channel1));
		Debug.Log ("set callback");
		Debug.Log (eog_set_callback (eog_callback, taskHandle1));
		Debug.Log ("callback set");
		Debug.Log ("second task");
		Debug.Log (taskHandle2 = eog_start_task (channel2));
		Debug.Log ("set callback");
		Debug.Log (eog_set_callback (eog_callback, taskHandle2));
		Debug.Log ("callback set");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Q)) {
			Debug.Log ("stop task");
			Debug.Log (eog_stop_task (taskHandle1));
			Debug.Log (eog_stop_task (taskHandle2));
		}
	}

	public void OnDestroy ()
	{
		//Application stopped running -- close() was called
		//applicationIsRunning = false;
		Debug.Log (eog_stop_task (taskHandle1));
		Debug.Log (eog_stop_task (taskHandle2));
		Debug.Log ("closed task");
	}
}