using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;

public class GetEyeData : MotherOfLogs 
{
	#if NIDAQ
	// this declares the callback (delegate) that we will be
	// calling from the C code
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void EOGCallbackDel(IntPtr taskHandle,
	                                      Int32 everyNSamplesEventType,
	                                      UInt32 nSamples,
	                                      IntPtr callbackData);

	// so we can send a string to know which channel is which
	[DllImport("NidaqPlugin", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr EOGStartTask(String channel);

	// import the c++ call to set the call back
	[DllImport ("NidaqPlugin")]
	private static extern int EOGSetCallback (
		[MarshalAs(UnmanagedType.FunctionPtr)]EOGCallbackDel 
		eyeData, IntPtr taskHandle);
	
	// [DllImport ("NidaqPlugin")]
	// private static extern Int32 eog_start_task();
	
	[DllImport ("NidaqPlugin")]
	private static extern Int32 EOGStopTask(IntPtr taskHandle);
	
	[DllImport ("NidaqPlugin")]
	private static extern Double[] EOGReturnData(IntPtr taskHandle);
	
	// private Int32 numSamples = 1;
	private Int32 DAQmx_Val_GroupByChannel;
	private Int32 read;
	// set this to a ridiculous number so sure we are getting data
	public Double[] data = new double[2];
	private IntPtr taskHandle1;
	private IntPtr taskHandle2;
	private String[] stringData;
	public String channel1 = "Dev1/ai3";
	public String channel2 = "Dev1/ai4";

	// Use this for initialization
	void Start () 
	{
		Debug.Log ("length of data at start");
		Debug.Log (data.Length);
		// this defines the callback that is called from the C++
		// code. 
		EOGCallbackDel EOGCallback =
			(IntPtr taskHandle, Int32 everyNSamplesEventType, UInt32 nSamples, IntPtr callbackData) =>
		{	
			try
			{
				Debug.Log ("in callback");
				data = EOGReturnData(taskHandle);
				// is it putting each number in a different element? but still too big!
				//Debug.Log (data.Length);
				Debug.Log (data[0]);
				Debug.Log (data[1]);

				/*
				for(int i = 0; i < data.Length; ++i)
				{
					stringData += data[i].ToString();
					if (i < data.Length)
					{
						stringData += ", ";
					}
				}
				*/
				//stringData = Array.ConvertAll(data, element => element.ToString());
				//Debug.Log(string.Join(", ", stringData));
				//Debug.Log (stringData);
				//eyeLog.Log (GameClock.Instance.SystemTime_Milliseconds, stringData);
			}
			catch (Exception ex)
			{
				Debug.Log ("Exception");
				Debug.Log (ex.Message);
				Debug.Log (ex.GetBaseException());
			}
		};
		Debug.Log ("start task");
		Debug.Log (taskHandle1 = EOGStartTask (channel1));
		Debug.Log (taskHandle1 = EOGStartTask (channel2));
		Debug.Log ("set callback");
		Debug.Log (EOGSetCallback (EOGCallback, taskHandle1));
		Debug.Log ("callback set");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.Q)) {
			Debug.Log ("stop task");
			Debug.Log (EOGStopTask (taskHandle1));
		}
	}

	public void OnDestroy ()
	{
		//Application stopped running -- close() was called
		//applicationIsRunning = false;
		Debug.Log (EOGStopTask (taskHandle1));
		Debug.Log ("closed task");
	}
	#endif
}