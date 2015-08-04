using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class GiveReward : MonoBehaviour {
	[DllImport ("ASimplePlugin")]
	private static extern int PrintANumber();

	[DllImport ("ASimplePlugin")]
	private static extern int reward(int on);
	
	// Use this for initialization
	void Start () {
		Debug.Log(PrintANumber());
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
}
