using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class FacebookImplementation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		FB.Init (InitCallback);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void InitCallback() {
		FB.LogInWithReadPermissions ("public_profile,email", LoginCallback);
	}

	void LoginCallback(ILoginResult result) {
		//handle result
		if (FB.IsLoggedIn) {
			FB.API("/me?fields=first_name", HttpMethod.GET, delegate (IGraphResult apiresult) {
				// Add error handling here
				if (apiresult.ResultDictionary != null) {
					string name;
					if (apiresult.ResultDictionary.TryGetValue("first_name", out name)) {
						//got name
					} else {
						name = "Player1";
					}
				}
			});
		}
	}

	void FacebookShare () {
		FB.ShareLink (
			"https://facebook.com",
			callback: ShareCallback);
	}

	void ShareCallback(IShareResult result) {
		//handle result
	}
}
