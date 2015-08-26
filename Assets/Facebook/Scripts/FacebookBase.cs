using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity
{
    internal abstract class FacebookBase : IFacebookImplementation
    {
        private IList<FacebookDelegate<ILoginResult>> authDelegates = new List<FacebookDelegate<ILoginResult>>();
        public abstract bool LimitEventUsage { get; set; }
        public abstract string FacebookSdkVersion { get; }

        protected CallbackManager CallbackManager { get; private set; }

        public bool LoggedIn
        {
            get
            {
                return AccessToken.CurrentAccessToken != null;
            }
        }

        protected FacebookBase(CallbackManager callbackManager)
        {
            this.CallbackManager = callbackManager;
        }

        public abstract void Init(
            InitDelegate onInitComplete,
            string appId,
            bool cookie,
            bool logging,
            bool status,
            bool xfbml,
            string channelUrl,
            string authResponse,
            bool frictionlessRequests,
            HideUnityDelegate hideUnityDelegate);

        public abstract void LogInWithPublishPermissions(
            string scope,
            FacebookDelegate<ILoginResult> callback);

        public abstract void LogInWithReadPermissions(
            string scope,
            FacebookDelegate<ILoginResult> callback);

        public virtual void LogOut()
        {
            AccessToken.CurrentAccessToken = null;
        }

        public void AppRequest(
            string message,
            string[] to = null,
            List<object> filters = null,
            string[] excludeIds = null,
            int? maxRecipients = null,
            string data = "",
            string title = "",
            FacebookDelegate<IAppRequestResult> callback = null)
        {
            AppRequest(message, null, null, to, filters, excludeIds, maxRecipients, data, title, callback);
        }

        public abstract void AppRequest(
            string message,
            OGActionType actionType,
            string objectId,
            string[] to,
            List<object> filters,
            string[] excludeIds,
            int? maxRecipients,
            string data,
            string title,
            FacebookDelegate<IAppRequestResult> callback);

        public abstract void ShareLink(
            string contentURL,
            string contentTitle,
            string contentDescription,
            string photoURL,
            FacebookDelegate<IShareResult> callback);

        public abstract void FeedShare(
            string toId,
            Uri link,
            string linkName,
            string linkCaption,
            string linkDescription,
            Uri picture,
            string mediaSource,
            FacebookDelegate<IShareResult> callback);

        public void API(
            string query,
            HttpMethod method,
            Dictionary<string, string> formData,
            FacebookDelegate<IGraphResult> callback)
        {
            Dictionary<string, string> inputFormData;
            // Copy the formData by value so it's not vulnerable to scene changes and object deletions
            inputFormData = (formData != null) ? CopyByValue(formData) : new Dictionary<string, string>();
            if (!inputFormData.ContainsKey(Constants.AccessTokenKey) && !query.Contains("access_token="))
            {
                inputFormData[Constants.AccessTokenKey] =
                    FB.IsLoggedIn ? AccessToken.CurrentAccessToken.TokenString : "";
            }

            AsyncRequestString.Request(GetGraphUrl(query), method, inputFormData, callback);
        }

        public void API(
            string query,
            HttpMethod method,
            WWWForm formData,
            FacebookDelegate<IGraphResult> callback)
        {

            if (formData == null)
            {
                formData = new WWWForm();
            }

            string tokenString = (AccessToken.CurrentAccessToken != null) ?
                AccessToken.CurrentAccessToken.TokenString : "";
            formData.AddField(
                Constants.AccessTokenKey,
                tokenString);

            AsyncRequestString.Request(GetGraphUrl(query), method, formData, callback);
        }

        public abstract void GameGroupCreate(
            string name,
            string description,
            string privacy,
            FacebookDelegate<IGroupCreateResult> callback);

        public abstract void GameGroupJoin(
            string id,
            FacebookDelegate<IGroupJoinResult> callback);

        public abstract void ActivateApp(string appId = null);

        public abstract void GetDeepLink(FacebookDelegate<IGetDeepLinkResult> callback);

        public abstract void AppEventsLogEvent(
            string logEvent,
            float? valueToSum,
            Dictionary<string, object> parameters);

        public abstract void AppEventsLogPurchase(
            float logPurchase,
            string currency,
            Dictionary<string, object> parameters);

        public abstract void OnInitComplete(string message);

        public abstract void OnLoginComplete(string message);

        public void OnLogoutComplete(string message)
        {
            AccessToken.CurrentAccessToken = null;
        }

        public abstract void OnGetDeepLinkComplete(string message);

        public abstract void OnGroupCreateComplete(string message);
        public abstract void OnGroupJoinComplete(string message);

        public abstract void OnAppRequestsComplete(string message);

        public abstract void OnShareLinkComplete(string message);

        protected void AddAuthDelegate(FacebookDelegate<ILoginResult> callback)
        {
            authDelegates.Add(callback);
        }

        private void GetAuthResponse(FacebookDelegate<ILoginResult> callback)
        {
            AddAuthDelegate(callback);
        }

        protected void ValidateAppRequestArgs(
            string message,
            OGActionType actionType,
            string objectId,
            string[] to = null,
            List<object> filters = null,
            string[] excludeIds = null,
            int? maxRecipients = null,
            string data = "",
            string title = "",
            FacebookDelegate<IAppRequestResult> callback = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message", "message cannot be null or empty!");
            }

            if (!string.IsNullOrEmpty(objectId) && !(actionType == OGActionType.AskFor || actionType == OGActionType.Send))
            {
                throw new ArgumentNullException("objectId", "Object ID must be set if and only if action type is SEND or ASKFOR");
            }

            if (actionType == null && !string.IsNullOrEmpty(objectId))
            {
                throw new ArgumentNullException("actionType", "You cannot provide an objectId without an actionType");
            }
        }

        protected void OnAuthResponse(ILoginResult result)
        {
            // If the login is cancelled we won't have a access token.
            // Don't overwrite a valid token
            if (result.AccessToken != null)
            {
                AccessToken.CurrentAccessToken = result.AccessToken;
            }

            foreach (FacebookDelegate<ILoginResult> callback in authDelegates)
            {
                if (callback != null)
                {
                    callback(result);
                }
            }
            authDelegates.Clear();
        }

        private Dictionary<string, string> CopyByValue(Dictionary<string, string> data)
        {
            var newData = new Dictionary<string, string>(data.Count);
            foreach (KeyValuePair<string, string> kvp in data)
            {
                newData[kvp.Key] = String.Copy(kvp.Value);
            }
            return newData;
        }

        private string GetGraphUrl(string query)
        {
            if (!query.StartsWith("/"))
            {
                query = "/" + query;
            }
            return Constants.GraphUrl + query;
        }
    }
}
