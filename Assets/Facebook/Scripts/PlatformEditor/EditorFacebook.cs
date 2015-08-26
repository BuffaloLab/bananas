using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Facebook.Unity.Editor
{
    internal class EditorFacebook : FacebookBase
    {
        private FacebookDelegate<LoginResult> loginCallback;

        public override bool LimitEventUsage { get; set; }

        public override string FacebookSdkVersion
        {
            get
            {
                return "None";
            }
        }

        public EditorFacebook() : base(new CallbackManager())
        {
        }

        public override void Init(
            InitDelegate onInitComplete,
            string appId,
            bool cookie,
            bool logging,
            bool status,
            bool xfbml,
            string channelUrl,
            string authResponse,
            bool frictionlessRequests,
            HideUnityDelegate hideUnityDelegate)
        {
            if (onInitComplete != null)
            {
                onInitComplete();
            }

            var editorFB = ComponentFactory.GetComponent<EditorFacebookGameObject>();
            editorFB.OnInitComplete("");
        }

        public override void LogInWithReadPermissions(
            string scope,
            FacebookDelegate<ILoginResult> callback)
        {
            AddAuthDelegate(callback);
            ComponentFactory.GetComponent<EditorFacebookAccessToken>();
        }

        public override void LogInWithPublishPermissions(
            string scope,
            FacebookDelegate<ILoginResult> callback)
        {
            AddAuthDelegate(callback);
            ComponentFactory.GetComponent<EditorFacebookAccessToken>();
        }

        public override void AppRequest(
            string message,
            OGActionType actionType,
            string objectId,
            string[] to ,
            List<object> filters,
            string[] excludeIds,
            int? maxRecipients ,
            string data,
            string title,
            FacebookDelegate<IAppRequestResult> callback)
        {
            FacebookLogger.Info("App Request dialog is not implemented in the Unity editor.");
        }

        public override void ShareLink(
            string contentURL,
            string contentTitle,
            string contentDescription,
            string photoURL, 
            FacebookDelegate<IShareResult> callback)
        {
            FacebookLogger.Info("Share Link is not implemented in the Unity editor.");
        }

        public override void FeedShare(
            string toId,
            Uri link,
            string linkName,
            string linkCaption,
            string linkDescription,
            Uri picture,
            string mediaSource,
            FacebookDelegate<IShareResult> callback)
        {
            Facebook.Unity.FacebookLogger.Info("Feed Share is not implemented in the Unity editor.");
        }

        public override void GameGroupCreate(
            string name,
            string description,
            string privacy,
            FacebookDelegate<IGroupCreateResult> callback)
        {
            throw new PlatformNotSupportedException("There is no Facebook GameGroupCreate Dialog on Editor");
        }

        public override void GameGroupJoin(
            string id,
            FacebookDelegate<IGroupJoinResult> callback)
        {
            throw new PlatformNotSupportedException("There is no Facebook GameGroupJoin Dialog on Editor");
        }

        public override void ActivateApp(string appId)
        {
            FacebookLogger.Info("This only needs to be called for iOS or Android.");
        }

        public override void GetDeepLink(FacebookDelegate<IGetDeepLinkResult> callback)
        {
            FacebookLogger.Info("No Deep Linking in the Editor");
        }

        public override void AppEventsLogEvent(
            string logEvent,
            float? valueToSum,
            Dictionary<string, object> parameters)
        {
            Debug.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
        }

        public override void AppEventsLogPurchase(
            float logPurchase,
            string currency,
            Dictionary<string, object> parameters)
        {
            Debug.Log("Pew! Pretending to send this off.  Doesn't actually work in the editor");
        }

        public override void OnAppRequestsComplete(string message)
        {
            throw new NotImplementedException();
        }

        public override void OnGetDeepLinkComplete(string message)
        {
            throw new NotImplementedException();
        }

        public override void OnGroupCreateComplete(string message)
        {
            throw new NotImplementedException();
        }

        public override void OnGroupJoinComplete(string message)
        {
            throw new NotImplementedException();
        }

        public override void OnInitComplete(string message)
        {
            // Do Nothing
        }

        public override void OnLoginComplete(string message)
        {
            throw new NotImplementedException();
        }

        public override void OnShareLinkComplete(string message)
        {
            throw new NotImplementedException();
        }
    }
}
