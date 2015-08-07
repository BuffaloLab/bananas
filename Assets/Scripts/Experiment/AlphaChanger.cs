using UnityEngine;
using System.Collections;

public class AlphaChanger : MonoBehaviour {
	float myAlpha = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetAlpha(float alpha){
		myAlpha = alpha;
	
		Renderer myMainRenderer = GetComponent<Renderer> ();
		if (myMainRenderer != null) {
			Color origColor = myMainRenderer.material.color;
			Color newColor = new Color (origColor.r, origColor.g, origColor.b, alpha); 
			myMainRenderer.material.color = newColor;
		}
	
		Renderer[] myRenderers = GetComponentsInChildren<Renderer> ();
		for (int i = 0; i < myRenderers.Length; i++) {
			Color origColor = myRenderers [i].material.color;
			Color newColor = new Color (origColor.r, origColor.g, origColor.b, alpha); 
			myRenderers [i].material.color = newColor;
		}
	
	}

	public float GetAlpha(){
		return myAlpha;
	}
}
