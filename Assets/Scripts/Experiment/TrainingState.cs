using UnityEngine;
using System.Collections;

public class TrainingState : MonoBehaviour {
	public enum Movement {
		forward,
		right,
		left,
		turn,
		full
	}
	public Movement Move;
}