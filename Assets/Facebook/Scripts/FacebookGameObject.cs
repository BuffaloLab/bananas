using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Facebook.Unity
{
    public delegate void InitDelegate();
    public delegate void FacebookDelegate<T>(T result) where T : IResult;
    public delegate void HideUnityDelegate(bool isUnityShown);

    internal abstract class FacebookGameObject : MonoBehaviour, IFacebookCallbackHandler
    {
        public IFacebookImplementation Facebook { get; set; }

        void Awake()
        {
            DontDestroyOnLoad(this);
            AccessToken.CurrentAccessToken = null;
            // run whatever else needs to be setup
            OnAwake();
        }

        // use this to call the rest of the Awake function
        protected virtual void OnAwake() {}

        public bool Initialized { get; private set; }

        public void OnInitComplete(string message)
        {
            this.Facebook.OnInitComplete(message);
            this.Initialized = true;
        }

        public void OnLoginComplete(string message)
        {
            this.Facebook.OnLoginComplete(message);
        }

        public void OnLogoutComplete(string message)
        {
            this.Facebook.OnLogoutComplete(message);
        }

        public void OnGetDeepLinkComplete(string message)
        {
            this.Facebook.OnGetDeepLinkComplete(message);
        }

        public void OnGroupCreateComplete(string message)
        {
            this.Facebook.OnGroupCreateComplete(message);
        }

        public void OnGroupJoinComplete(string message)
        {
            this.Facebook.OnGroupJoinComplete(message);
        }

        public void OnAppRequestsComplete(string message)
        {
            this.Facebook.OnAppRequestsComplete(message);
        }

        public void OnShareLinkComplete(string message)
        {
            this.Facebook.OnShareLinkComplete(message);
        }
    }
}
