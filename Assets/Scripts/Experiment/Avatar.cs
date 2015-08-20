using UnityEngine;
using System.Collections;

public abstract class Avatar: MonoBehaviour{

	public Experiment exp;
	public GiveReward reward;

	public bool ShouldLockControls = false;
	public float driveSpeed = 5.0f;

	public float RotationSpeed = 1;

	// Use this for initialization
	void Start () {
		exp = GameObject.FindGameObjectWithTag ("Experiment").GetComponent<Experiment>();
		reward = GameObject.FindGameObjectWithTag ("Reward").GetComponent<GiveReward> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(!exp.isReplay){
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY; // TODO: on collision, don't allow a change in angular velocity?
			GetComponent<Collider>().enabled = true;
			if (reward.isFrozen){
				GetComponent<Rigidbody>().velocity = Vector3.zero;
				GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			}else{
				GetInput ();
			}
		}
		else{
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
			GetComponent<Collider>().enabled = false;
		}
	}

	public abstract void GetInput();
}
