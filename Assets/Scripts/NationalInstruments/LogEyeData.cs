using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Linq;

public class GetEyeData
{
	LoggerQueue myLoggerQueue;

	//public string fileName;

	#if NIDAQ
	// public Logger_Threading eyeLog{get{return LogController.Instance.eyeLog;}}
	// this declares the callback (delegate) that we will be
	// calling from the C code
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	public delegate void EOGCallbackDel(IntPtr taskHandle,
	                                      Int32 everyNSamplesEventType,
	                                      UInt32 nSamples,
	                                      IntPtr callbackData);

	// so we can send a string to know which channel is which
	[DllImport("NidaqPlugin", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr EOGSetChannel();

	// start task
	[DllImport("NidaqPlugin", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr EOGStartTask();

	// import the c++ call to set the call back
	[DllImport ("NidaqPlugin")]
	private static extern IntPtr EOGSetCallback (
		[MarshalAs(UnmanagedType.FunctionPtr)]EOGCallbackDel 
		eyeData);
	
	[DllImport ("NidaqPlugin")]
	private static extern Int32 EOGStopTask(IntPtr taskHandle);
	
	[DllImport ("NidaqPlugin")]
	private static extern IntPtr EOGReturnData(IntPtr taskHandle);
	
	public Double[] newData = new double[2];
	private IntPtr taskHandle1;
	private String stringData;
	// no idea why I can't set the second channel. Give up. Hard coding in C++ :(
	// public String channel1 = "Dev1/ai3";
	// public String channel2 = "Dev1/ai4";

	public void StartData()
	{
        // this defines the callback that is called from the C++
        // code. 
        EOGCallbackDel EOGCallback =
            (IntPtr taskHandle, Int32 everyNSamplesEventType, UInt32 nSamples, IntPtr callbackData) =>
            {
                //Debug.Log("in callback");
                IntPtr ptrData = EOGReturnData(taskHandle);
                // Debug.Log ("in callback");

                Marshal.Copy(ptrData, newData, 0, 2);

                // Debug.Log (newData[0]);
                // Debug.Log (newData[1]);

                stringData = String.Join(",", newData.Select(p => p.ToString()).ToArray());

                //Debug.Log ( stringData );

                // put in queue here
                myLoggerQueue.AddToLogQueue(GameClock.Instance.SystemTime_Milliseconds + "," + stringData);
                //eyeLog.Log (GameClock.Instance.SystemTime_Milliseconds, stringData);

                // where do we pull from the queue?

                // Free the unmanaged memory.
                //Marshal.FreeHGlobal(ptrData);
            };
       
        Debug.Log ("start task");
        taskHandle1 = EOGStartTask();
        Debug.Log ("set channels");
        Debug.Log(taskHandle1);
        taskHandle1 = EOGSetChannel();
        Debug.Log ("set callback");
        Debug.Log(taskHandle1);
        taskHandle1 = EOGSetCallback(EOGCallback);
        Debug.Log ("callback set");
        Debug.Log(taskHandle1);
    }

    public LoggerQueue StartQueue()
    {
        myLoggerQueue = new LoggerQueue();
        return myLoggerQueue;
    }

    public void Close()
	{
		EOGStopTask (taskHandle1);
	}
	#endif

}
public class LogEyeData : MonoBehaviour 
{
	#if NIDAQ
	public string fileName;
	GetEyeData newEyeData;
    LoggerQueue myLoggerQueue;
    LoggerWriter myLoggerWriter;

    public void StartLogging(string file)
    {
        fileName = file;
        Debug.Log("get file name");
        newEyeData = new GetEyeData();
        Debug.Log("start logeyedata");
        Debug.Log(fileName);
        myLoggerQueue = newEyeData.StartQueue();
        myLoggerWriter = new LoggerWriter(fileName, myLoggerQueue);
        myLoggerWriter.Start();
        myLoggerWriter.log("DATE: " + DateTime.Now.ToString("M/d/yyyy, " + GameClock.Instance.SystemTime_Milliseconds));
        newEyeData.StartData();
    }

	public LogEyeData(string file){
        fileName = file;
        Debug.Log("get file name");
    }
   
    void Update()
    {
        if (myLoggerWriter != null)
        {
            //Debug.Log("log writer working");
            //Debug.Log(myLoggerQueue.logQueue.Count);
            if (myLoggerWriter.Update())
            {
                //Debug.Log("log");
                // Alternative to the OnFinished callback
                myLoggerWriter = null;
            }
        }
    }
    
    //must be called by the LogController class OnApplicationQuit()
    public void close()
	{
		//Application stopped running -- close() was called
		//applicationIsRunning = false;
		newEyeData.Close();
        myLoggerWriter.log("DATE: " + DateTime.Now.ToString("M/d/yyyy, " + GameClock.Instance.SystemTime_Milliseconds));
        myLoggerWriter.End();
    }
    
	#endif
}