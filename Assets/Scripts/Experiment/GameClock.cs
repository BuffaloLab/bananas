using UnityEngine;
using System.Collections;
using System;

public class GameClock : MonoBehaviour {

	public long SystemTime_Milliseconds { get { return GetSystemClockMilliseconds (); } }

	//SINGLETON
	private static GameClock _instance;
	
	public static GameClock Instance{
		get{
			return _instance;
		}
	}
	
	void Awake(){
		
		if (_instance != null) {
			Debug.Log("Instance already exists!");
			Destroy(transform.gameObject);
			return;
		}
		_instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	long GetSystemClockMilliseconds(){
		long tick = DateTime.Now.Ticks;
		//Debug.Log (DateTime.Now.Ticks);
		//Debug.Log (DateTime.Now);
		
		//long seconds = tick / TimeSpan.TicksPerSecond;
		long milliseconds = tick / TimeSpan.TicksPerMillisecond;

		return milliseconds;
	}

}
