using UnityEngine;
using System.Collections.Generic;

namespace Facebook.Unity.Canvas
{
    internal class JsBridge : MonoBehaviour
    {
        private ICanvasFacebookCallbackHandler facebook;

        void Start()
        {
            facebook = ComponentFactory.GetComponent<CanvasFacebookGameObject>(ComponentFactory.IfNotExist.ReturnNull);
        }

        void OnFacebookAuthResponse(string responseJsonData = "")
        {
            facebook.OnFacebookAuthResponse(responseJsonData);
        }

        void OnFacebookAuthResponseChange(string responseJsonData = "")
        {
            facebook.OnFacebookAuthResponseChange(responseJsonData);
        }

        void OnPayComplete(string responseJsonData = "")
        {
            facebook.OnPayComplete(responseJsonData);
        }

        void OnAppRequestsComplete(string responseJsonData = "")
        {
            facebook.OnAppRequestsComplete(responseJsonData);
        }

        void OnShareLinkComplete(string responseJsonData = "")
        {
            facebook.OnShareLinkComplete(responseJsonData);
        }

        void OnGroupCreateComplete(string responseJsonData = "")
        {
            facebook.OnGroupCreateComplete(responseJsonData);;
        }

        void OnJoinGroupComplete(string responseJsonData = "")
        {
            facebook.OnGroupJoinComplete(responseJsonData);
        }

        void OnFacebookFocus(string state)
        {
            facebook.OnHideUnity((state != "hide"));
        }

        void OnInitComplete(string responseJsonData = "")
        {
            facebook.OnInitComplete(responseJsonData);
        }

        void OnUrlResponse(string url = "")
        {
            facebook.OnUrlResponse(url);
        }
    }
}
