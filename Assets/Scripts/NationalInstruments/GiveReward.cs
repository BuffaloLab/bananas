using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Reflection;

public class GiveReward : MonoBehaviour {
	public bool isFrozen;
	public AudioSource beepSound;

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
		//isFrozen = false;
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

		if (Input.GetKeyDown (KeyCode.Space)) {
			RewardAndGo(1);
		}
	}

	public void RewardAndFreeze(int numBeeps){
		StartCoroutine (GiveBeeps (numBeeps,true));
	}

	public void RewardAndGo(int numBeeps){
		StartCoroutine(GiveBeeps (numBeeps,false));
	}

	IEnumerator GiveBeeps(int numBeeps, bool freezeMe){
		beepSound.Play();
		isFrozen = freezeMe;
		for (int i = 0; i<numBeeps; i++) {
			print ("BEEP! " + i);
			Reward (1);
			yield return new WaitForSeconds (.2f);
			Reward (0);
			yield return new WaitForSeconds (.05f);
		}
		isFrozen = false;
	}
}