using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity.Example
{
    internal class GraphRequest : MenuBase
    {
        private string apiQuery = "";

        protected override void getGui()
        {
            bool enabled = GUI.enabled;
            GUI.enabled = enabled && FB.IsLoggedIn;
            if (Button("Basic Request - Me"))
            {
                FB.API("/me", HttpMethod.GET, handleResult);
            }

            if (Button("Take and Upload screenshot"))
            {
                StartCoroutine(TakeScreenshot());
            }

            LabelAndTextField("Request", ref apiQuery);
            if (Button("Custom Request"))
            {
                FB.API(apiQuery, HttpMethod.GET, handleResult);
            }
            GUI.enabled = enabled;
        }

        private IEnumerator TakeScreenshot()
        {
            yield return new WaitForEndOfFrame();

            var width = Screen.width;
            var height = Screen.height;
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            // Read screen contents into the texture
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            byte[] screenshot = tex.EncodeToPNG();

            var wwwForm = new WWWForm();
            wwwForm.AddBinaryData("image", screenshot, "InteractiveConsole.png");
            wwwForm.AddField("message", "herp derp.  I did a thing!  Did I do this right?");
            FB.API("me/photos", HttpMethod.POST, handleResult, wwwForm);
        }
    }
}
