using System;
using UnityEngine;

namespace Facebook.Unity.Canvas
{
    internal class CanvasFacebookGameObject : FacebookGameObject, ICanvasFacebookCallbackHandler
    {
        protected ICanvasFacebookImplementation CanvasFacebookImpl
        {
            get
            {
                return (ICanvasFacebookImplementation) this.Facebook;
            }
        }

        protected override void OnAwake()
        {
            // Facebook JS Bridge lives in it's own gameobject for optimization reasons
            // see UnityObject.SendMessage()
            var bridgeObject = new GameObject("FacebookJsBridge");
            bridgeObject.AddComponent<JsBridge>();
            bridgeObject.transform.parent = gameObject.transform;
        }

        public void OnPayComplete(string result)
        {
            this.CanvasFacebookImpl.OnPayComplete(result);
        }

        public void OnFacebookAuthResponseChange(string message)
        {
            this.CanvasFacebookImpl.OnFacebookAuthResponseChange(message);
        }

        public void OnFacebookAuthResponse(string message)
        {
            this.CanvasFacebookImpl.OnFacebookAuthResponse(message);
        }

        public void OnUrlResponse(string message)
        {
            this.CanvasFacebookImpl.OnUrlResponse(message);
        }

        public void OnHideUnity(bool hide)
        {
            this.CanvasFacebookImpl.OnHideUnity(hide);
        }
    }
}
