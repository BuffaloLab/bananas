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
	public delegate void eog_callback_del(IntPtr taskHandle,
	                                      Int32 everyNSamplesEventType,
	                                      UInt32 nSamples,
	                                      IntPtr callbackData);
	
	// import the c++ call to set the call back
	[DllImport ("NidaqPlugin")]
	private static extern int eog_set_callback (
		[MarshalAs(UnmanagedType.FunctionPtr)]eog_callback_del 
		eye_data);
	
	[DllImport ("NidaqPlugin")]
	private static extern Int32 eog_start_task();
	
	[DllImport ("NidaqPlugin")]
	private static extern Int32 eog_stop_task();
	
	[DllImport ("NidaqPlugin")]
	private static extern Double eog_return_data();
	
	public Int32 numSamples = 1;
	public Int32 DAQmx_Val_GroupByChannel;
	public Int32 read;
	// set this to a ridiculous number so sure we are getting data
	public Double data = 20;

	// Use this for initialization
	void Start () 
	{
		// this defines the callback that is called from the C++
		// code. 
		eog_callback_del eog_callback =
			(IntPtr taskHandle, Int32 everyNSamplesEventType, UInt32 nSamples, IntPtr callbackData) =>
		{	
			Debug.Log ("made callback");
			Debug.Log (data);
			try
			{
				Debug.Log (data = eog_return_data());
			}
			catch (Exception ex)
			{
				Debug.Log ("Exception");
				Debug.Log (ex.Message);
				Debug.Log (ex.GetBaseException());
			}
		};
		Debug.Log ("start task");
		Debug.Log (DAQmx_Val_GroupByChannel = eog_start_task ());
		Debug.Log ("set callback");
		Debug.Log (eog_set_callback (eog_callback));
		Debug.Log ("callback set");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Q)) {
			Debug.Log ("stop task");
			Debug.Log (eog_stop_task ());
		}
	}
}