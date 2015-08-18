using UnityEngine;
using System.Collections;

public abstract class Avatar: MonoBehaviour{

	public Experiment exp;

	public bool ShouldLockControls = false;
	public float driveSpeed = 5.0f;

	public float RotationSpeed = 1;

	// Use this for initialization
	void Start () {
		exp = GameObject.FindGameObjectWithTag ("Experiment").GetComponent<Experiment>();
	}
	
	// Update is called once per frame
	void Update () {
		if(!exp.isReplay){
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY; // TODO: on collision, don't allow a change in angular velocity?
			GetComponent<Collider>().enabled = true;
			GetInput ();
		}
		else{
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			GetComponent<Collider>().enabled = false;
		}
	}

	public abstract void GetInput();
}
