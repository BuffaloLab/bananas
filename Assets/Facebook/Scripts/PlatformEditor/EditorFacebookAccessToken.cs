using System;
using System.Collections;
using System.Collections.Generic;
using Facebook;
using UnityEngine;

namespace Facebook.Unity.Editor
{
    internal class EditorFacebookAccessToken : MonoBehaviour
    {
        private const float windowWidth = 592;
        private float windowHeight = 200;
        private string accessToken = "";

        private bool isLoggingIn = false;

        private GUIStyle greyButton;

        void OnGUI()
        {
            var windowTop = Screen.height / 2 - windowHeight / 2;
            var windowLeft = Screen.width / 2 - windowWidth / 2;
            greyButton = GUI.skin.button;
            GUI.ModalWindow(GetHashCode(), new Rect(windowLeft, windowTop, windowWidth, windowHeight), OnGUIDialog, "Unity Editor Facebook Login");
        }

        private void OnGUIDialog(int windowId)
        {
            GUI.enabled = !isLoggingIn;
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Space(10);
            GUILayout.Label("User Access Token:");
            GUILayout.EndVertical();
            accessToken = GUILayout.TextField(accessToken, GUI.skin.textArea, GUILayout.MinWidth(400));
            GUILayout.EndHorizontal();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Find Access Token"))
            {
                Application.OpenURL(string.Format("https://developers.facebook.com/tools/accesstoken/?app_id={0}", FB.AppId));
            }
            GUILayout.FlexibleSpace();
            var loginLabel = new GUIContent("Login");
            var buttonRect = GUILayoutUtility.GetRect(loginLabel, GUI.skin.button);
            if (GUI.Button(buttonRect, loginLabel))
            {
                var facebook = ComponentFactory.GetComponent<EditorFacebookGameObject>();
                AccessToken.CurrentAccessToken = new AccessToken(accessToken, "editor mock", DateTime.Now, new List<string>());
                var formData = new Dictionary<string, string>();
                formData["batch"] = "[{\"method\":\"GET\", \"relative_url\":\"me?fields=id\"},{\"method\":\"GET\", \"relative_url\":\"app?fields=id\"}]";
                formData["method"] = "POST";
                formData["access_token"] = accessToken;
                FB.API("/", HttpMethod.GET, facebook.MockLoginCallback, formData);
                isLoggingIn = true;
            }
            GUI.enabled = true;
            var cancelLabel = new GUIContent("Cancel");
            var cancelButtonRect = GUILayoutUtility.GetRect(cancelLabel, greyButton);
            if (GUI.Button(cancelButtonRect, cancelLabel, greyButton))
            {
                ComponentFactory.GetComponent<EditorFacebookGameObject>().MockCancelledLoginCallback();
                Destroy(this);
            }
            GUILayout.EndHorizontal();

            if (Event.current.type == EventType.Repaint)
            {
                windowHeight = (cancelButtonRect.y + cancelButtonRect.height + GUI.skin.window.padding.bottom);
            }
        }
    }
}
