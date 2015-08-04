using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class Food : MonoBehaviour {

	public int IDnum;

	public void Initialize(){ //should be called from FoodLogger
	}

	void Start(){

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
	public void SetIDNum(int newID){
		IDnum = newID;
	}
}
