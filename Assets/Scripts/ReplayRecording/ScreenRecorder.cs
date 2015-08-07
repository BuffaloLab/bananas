using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScreenRecorder : MonoBehaviour {
	Experiment exp;
	
	int numFrames = 0;

	public enum RecordingType{
		screenshot,
		continuousVideo
	}

	void Start(){
		exp = GameObject.FindGameObjectWithTag ("Experiment").GetComponent<Experiment>();
	}

	public void TakeNextContinuousScreenShot(){
		StartCoroutine(TakeScreenshot(RecordingType.continuousVideo));
	}

	public string ScreenShotName(int width, int height, RecordingType recordingType) {
		string name = "";

		if (recordingType == RecordingType.screenshot) {
			name = string.Format ("{0}/screenshots/screen_{1}x{2}_{3}.png", 
		                     Application.dataPath, 
		                     width, height, 
		                     System.DateTime.Now.ToString ("yyyy-MM-dd_HH-mm-ss"));
		}
		else if (recordingType == RecordingType.continuousVideo) {
			name = string.Format ("{0}/screen_{1}x{2}_{3}.png", 
			                      exp.PNGRecordingPath, //change to path variable? 
			                      width, height, 
			                      numFrames);

			numFrames++;
		}

		return name;
	}
	
	//despite waiting for the end of frame, this coroutine will be started every fixed update, resulting in an image for every fixedupdate call. which is good, because video will have a constant framerate, whereas the game will not.
	IEnumerator TakeScreenshot(RecordingType recordingType){
		// We should only read the screen buffer after rendering is complete

		yield return new WaitForEndOfFrame();
		
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		
		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		
		// Encode texture into PNG
		byte[] bytes = tex.EncodeToPNG();
		Destroy(tex);
		
		// For testing purposes, also write to a file in the project folder
		string filename = ScreenShotName(width, height, recordingType);
		System.IO.File.WriteAllBytes(filename, bytes);

		yield return null;
	}

}
