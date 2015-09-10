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
	private static extern IntPtr EOGReturnData(IntPtr taskHandle);

	private Int32 read;
	// initialize data
	//public Double[] data = new double[2];
	// Initialize unmanged memory to hold the array.
	//static int size = Marshal.SizeOf(newData[0]) * newData.Length;
	//IntPtr pnt = Marshal.AllocHGlobal(size);

	public Double[] newData = new double[2];
	//public Int32[] newData = new Int32[2];
	private IntPtr taskHandle1;
	private String[] stringData;
	public String channel1 = "Dev1/ai3";
	public String channel2 = "Dev1/ai4";

	// Use this for initialization
	void Start () 
	{
		// this defines the callback that is called from the C++
		// code. 
		EOGCallbackDel EOGCallback =
			(IntPtr taskHandle, Int32 everyNSamplesEventType, UInt32 nSamples, IntPtr callbackData) =>
		{	
			IntPtr ptrData = EOGReturnData(taskHandle);
			Debug.Log ("in callback");
			try
			{
				//ptrData = EOGReturnData(taskHandle);
				//Debug.Log ("got data");
				Marshal.Copy ( ptrData, newData, 0, 2);
				//Debug.Log (ptrData[0]);
				//Debug.Log (ptrData[1]);
				// ugh. data is still not transferring correctly. 
			
				// is it putting each number in a different element? but still too big!
				//Debug.Log (data.Length);
				//Debug.Log ("get length");
				//Debug.Log (newData.GetLength(0));
				//EOGClearData(ptrData);
				//Debug.Log ("cleared data");

				Debug.Log (newData[0]);
				Debug.Log (newData[1]);
			
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
			finally 
			{
				// Free the unmanaged memory.
				Marshal.FreeHGlobal(ptrData);
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