using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class Food : MonoBehaviour {
	
	float myAlpha = 1.0f;
	Material myMaterial;

	void Start(){
		//myMaterial = 
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//get name without "(clone)" attached to it
	public string GetName(){
		string name = gameObject.name;
		name = Regex.Replace( name, "(Clone)", "" );
		name = Regex.Replace( name, "[()]", "" );
		
		return name;
	}

	//should be set when spawned by the FoodController
	public void SetNameID(int ID){
		if (ID < 10) {
			gameObject.name = gameObject.name + "00" + ID; 
		}
		else if(ID < 100) {
			gameObject.name = gameObject.name + "0" + ID; 
		}
		else if(ID < 1000) {
			gameObject.name = gameObject.name + ID; 
		}
	}

	public void SetAlpha(float alpha){
		myAlpha = alpha;

		Renderer myMainRenderer = GetComponent<Renderer> ();
		if (myMainRenderer != null) {
			Color origColor = myMainRenderer.material.color;
			Color newColor = new Color(origColor.r, origColor.g, origColor.b, alpha); 
			myMainRenderer.material.color = newColor;
		}

		Renderer[] myRenderers = GetComponentsInChildren<Renderer> ();
		for (int i = 0; i < myRenderers.Length; i++) {
			Color origColor = myRenderers[i].material.color;
			Color newColor = new Color(origColor.r, origColor.g, origColor.b, alpha); 
			myRenderers[i].material.color = newColor;
		}
	}

	public float GetAlpha(){
		return myAlpha;
	}
}
