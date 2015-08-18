using UnityEngine;
using System.Collections;

public class DrawCrosshair : MonoBehaviour {

	public Texture2D crosshairImage;
	public float scale=6;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		float xMin = (Screen.width / 2) - (crosshairImage.width / (2*scale));
		float yMin = (Screen.height / 2) - (crosshairImage.height / (2*scale));
		GUI.DrawTexture (new Rect (xMin, yMin, crosshairImage.width/scale, crosshairImage.height/scale), crosshairImage);
	}
}
