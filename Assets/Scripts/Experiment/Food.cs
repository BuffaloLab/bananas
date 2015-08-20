using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Diagnostics;

public class Food : MonoBehaviour {

	AlphaChanger myAlphaChanger;

	Material myMaterial;

	Stopwatch beenWaiting = new Stopwatch();


	void Start(){
		myAlphaChanger = gameObject.GetComponent<AlphaChanger> ();
		SetAlpha (1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		//NOTE: testing alpha changing/logging
		if (Input.GetKeyDown (KeyCode.Q)) {
			if(myAlphaChanger.GetAlpha() == 1.0f){
				SetAlpha(0.5f);
			}
			else{
				SetAlpha(1.0f);
			}
		}
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
		if (myAlphaChanger == null) {
			myAlphaChanger = GetComponent<AlphaChanger>();
		}

		if (myAlphaChanger != null) {
			myAlphaChanger.SetAlpha (alpha);

			FoodLogger myFoodLogger = GetComponent<FoodLogger>();
			if(myFoodLogger != null){
				myFoodLogger.LogAlpha(alpha);
			}
		}
		else {
			//UnityEngine.Debug.Log("No alpha changer attached.");
		}
	}

	void OnDestroy(){
	/*	beenWaiting.Reset ();
		while (beenWaiting.ElapsedMilliseconds<200) {
			print ("Wait!!!!!!!!!!");
			yield return null;
		}
		beenWaiting.Stop();
*/
		print ("Death to Fruit.");
	}
}
