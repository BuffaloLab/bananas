using UnityEngine;
using System.Collections;

public class DrawCrosshair : MonoBehaviour {
	Experiment exp;

	public Texture2D crosshairImage;
	public float scale=3;

	// Use this for initialization
	void Start () {
		exp = GameObject.FindGameObjectWithTag ("Experiment").GetComponent<Experiment> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		if (exp.drawCrosshair) {
			float xMin = (Screen.width / 2) - (crosshairImage.width / (2 * scale));
			float yMin = (Screen.height / 2) - (crosshairImage.height / (2 * scale));
			GUI.DrawTexture (new Rect (xMin, yMin, crosshairImage.width / scale, crosshairImage.height / scale), crosshairImage);
		}
	}
}
