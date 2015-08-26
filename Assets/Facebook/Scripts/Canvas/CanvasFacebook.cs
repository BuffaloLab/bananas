using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity.Canvas
{
    internal sealed class CanvasFacebook : FacebookBase, ICanvasFacebookImplementation
    {
        internal const string MethodAppRequests = "apprequests";
        internal const string MethodFeed = "feed";
        internal const string MethodPay = "pay";
        internal const string MethodGameGroupCreate = "game_group_create";
        internal const string MethodGameGroupJoin = "game_group_join";
        internal const string CancelledResponse = "{\"cancelled\":true}";
        internal const string FacebookConnectURL = "https://connect.facebook.net";
        internal const string SDKVersion = "v2.4";

        // The source code for our js sdk binding.
        private const string JSSDKBindingFileName = "JSSDKBindings";
        private const string sdkLocale = "en_US";

        private InitDelegate onInitComplete;
        private HideUnityDelegate OnHideUnityDelegate;

        private string appId;
        private bool sdkDebug = false;
        private string deepLink;

        #region Facebook API

        public override bool LimitEventUsage { get; set; }

        public override string FacebookSdkVersion
        {
            get
            {
                return String.Format("Facebook.JS.SDK.{0}", SDKVersion);
            }
        }

        public CanvasFacebook()
            : this(new CallbackManager())
        {
        }

        public CanvasFacebook(CallbackManager callbackManager)
            : base(callbackManager)
        {
        }

        private static string IntegrationMethodJs
        {
            get
            {
                TextAsset ta = Resources.Load(JSSDKBindingFileName) as TextAsset;
                if (ta)
                {
                    return ta.text;
                }

                return null;
            }
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
            if (string.IsNullOrEmpty(appId))
            {
                throw new ArgumentException("appId cannot be null or empty!");
            }

            if (CanvasFacebook.IntegrationMethodJs == null)
            {
                throw new Exception("Cannot initialize facebook javascript");
            }

            this.onInitComplete = onInitComplete;
            this.OnHideUnityDelegate = hideUnityDelegate;
            Application.ExternalEval(CanvasFacebook.IntegrationMethodJs);
            this.appId = appId;

            bool isPlayer = true;
            #if UNITY_WEBGL
            isPlayer = false;
            #endif

            MethodArguments parameters = new MethodArguments();
            parameters.addNonNullOrEmptyParameter("appId", appId);
            parameters.addNonNullParameter("cookie", cookie);
            parameters.addNonNullParameter("logging", logging);
            parameters.addNonNullParameter("status", status);
            parameters.addNonNullParameter("xfbml", xfbml);
            parameters.addNonNullOrEmptyParameter("channelUrl", channelUrl);
            parameters.addNonNullOrEmptyParameter("authResponse", authResponse);
            parameters.addNonNullParameter("frictionlessRequests", frictionlessRequests);
            parameters.addNonNullOrEmptyParameter("version", SDKVersion);
            // use 1/0 for booleans, otherwise you'll get strings "True"/"False"
            Application.ExternalCall(
                "FBUnity.init",
                isPlayer ? 1 : 0,
                FacebookConnectURL,
                sdkLocale,
                sdkDebug ? 1 : 0,
                parameters.ToJsonString(),
                status ? 1 : 0);

        }

        public override void LogInWithPublishPermissions(
            string scope,
            FacebookDelegate<ILoginResult> callback)
        {
            if (Screen.fullScreen)
            {
                Screen.fullScreen = false;
            }
            
            AddAuthDelegate(callback);
            Application.ExternalCall("FBUnity.login", scope);
        }

        public override void LogInWithReadPermissions(
            string scope,
            FacebookDelegate<ILoginResult> callback)
        {
            if (Screen.fullScreen)
            {
                Screen.fullScreen = false;
            }

            AddAuthDelegate(callback);
            Application.ExternalCall("FBUnity.login", scope);
        }

        public override void LogOut()
        {
            base.LogOut();
            Application.ExternalCall("FBUnity.logout");
        }

        public override void AppRequest(
                string message,
                OGActionType actionType,
                string objectId,
                string[] to,
                List<object> filters,
                string[] excludeIds,
                int? maxRecipients,
                string data,
                string title,
                FacebookDelegate<IAppRequestResult> callback)
        {
            ValidateAppRequestArgs(
                message,
                actionType,
                objectId,
                to,
                filters,
                excludeIds,
                maxRecipients,
                data,
                title,
                callback
            );

            MethodArguments args = new MethodArguments();
            args.addNonNullOrEmptyParameter("message", message);
            args.addCommaSeperateListNonNull("to", to);
            args.addNonNullOrEmptyParameter("action_type", actionType != null ? actionType.ToString() : null);
            args.addNonNullOrEmptyParameter("object_id", objectId);
            args.addNonNullParameter("filters", filters);
            args.addNonNullParameter("exclude_ids", excludeIds);
            args.addNonNullOrEmptyParameter("max_recipients", maxRecipients);
            args.addNonNullOrEmptyParameter("data", data);
            args.addNonNullOrEmptyParameter("title", title);
            var call = new CanvasUIMethodCall<IResult>(this, MethodAppRequests, Constants.OnAppRequestsCompleteMethodName);
            call.call(args);
        }
        
        public override void ActivateApp(string appId)
        {
            Application.ExternalCall("FBUnity.activateApp");
        }

        public override void ShareLink(
            string contentURL,
            string contentTitle,
            string contentDescription,
            string photoURL, 
            FacebookDelegate<IShareResult> callback)
        {
            MethodArguments args = new MethodArguments();
            args.addNonNullOrEmptyParameter("link", contentURL);
            args.addNonNullOrEmptyParameter("name", contentTitle);
            args.addNonNullOrEmptyParameter("description", contentDescription);
            args.addNonNullOrEmptyParameter("picture", photoURL);
            var call = new CanvasUIMethodCall<IShareResult>(this, MethodFeed, Constants.OnShareCompleteMethodName);
            call.Callback = callback;
            call.call(args);
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
            MethodArguments args = new MethodArguments();
            args.addNonNullOrEmptyParameter("to", toId);
            args.addNonNullOrEmptyParameter("link", link);
            args.addNonNullOrEmptyParameter("name", linkName);
            args.addNonNullOrEmptyParameter("caption", linkCaption);
            args.addNonNullOrEmptyParameter("description", linkDescription);
            args.addNonNullOrEmptyParameter("picture", picture);
            args.addNonNullOrEmptyParameter("source", mediaSource);
            var call = new CanvasUIMethodCall<IShareResult>(this, MethodFeed, Constants.OnShareCompleteMethodName);
            call.Callback = callback;
            call.call(args);
        }

        public void Pay(
            string product,
            string action,
            int quantity,
            int? quantityMin,
            int? quantityMax,
            string requestId,
            string pricepointId,
            string testCurrency,
            FacebookDelegate<IPayResult> callback)
        {
            MethodArguments args = new MethodArguments();
            args.addNonNullOrEmptyParameter("product", product);
            args.addNonNullOrEmptyParameter("action", action);
            args.addNonNullOrEmptyParameter("quantity", quantity);
            args.addNonNullOrEmptyParameter("quantity_min", quantityMin);
            args.addNonNullOrEmptyParameter("quantity_max", quantityMax);
            args.addNonNullOrEmptyParameter("request_id", requestId);
            args.addNonNullOrEmptyParameter("pricepoint_id", pricepointId);
            args.addNonNullOrEmptyParameter("test_currency", testCurrency);
            var call = new CanvasUIMethodCall<IPayResult>(this, MethodPay, Constants.OnPayCompleteMethodName);
            call.Callback = callback;
            call.call(args);
        }

        public override void GameGroupCreate(
            string name,
            string description,
            string privacy,
            FacebookDelegate<IGroupCreateResult> callback)
        {
            MethodArguments args = new MethodArguments();
            args.addNonNullOrEmptyParameter("name", name);
            args.addNonNullOrEmptyParameter("description", description);
            args.addNonNullOrEmptyParameter("privacy", privacy);
            args.addNonNullOrEmptyParameter("display", "async");
            var call = new CanvasUIMethodCall<IGroupCreateResult>(this, MethodGameGroupCreate, Constants.OnGroupCreateCompleteMethodName);
            call.Callback = callback;
            call.call(args);
        }

        public override void GameGroupJoin(
            string id,
            FacebookDelegate<IGroupJoinResult> callback)
        {
            MethodArguments args = new MethodArguments();
            args.addNonNullOrEmptyParameter("id", id);
            args.addNonNullOrEmptyParameter("display", "async");
            var call = new CanvasUIMethodCall<IGroupJoinResult>(this, MethodGameGroupJoin, Constants.OnJoinGroupCompleteMethodName);
            call.Callback = callback;
            call.call(args);
        }

        public override void GetDeepLink(FacebookDelegate<IGetDeepLinkResult> callback)
        {
            if (callback != null)
            {
                callback(new GetDeepLinkResult(deepLink));
                deepLink = "";
            }
        }

        public override void AppEventsLogEvent(
            string logEvent,
            float? valueToSum,
            Dictionary<string, object> parameters)
        {
            Application.ExternalCall(
                "FBUnity.logAppEvent",
                logEvent,
                valueToSum,
                MiniJSON.Json.Serialize(parameters)
            );
        }

        public override void AppEventsLogPurchase(
            float logPurchase,
            string currency,
            Dictionary<string, object> parameters)
        {
            Application.ExternalCall(
                "FBUnity.logPurchase",
                logPurchase,
                currency,
                MiniJSON.Json.Serialize(parameters)
            );
        }

        #endregion

        #region Facebook JS Bridge calls
        public void OnFacebookAuthResponse(string responseJsonData)
        {
            var loginStatus = MiniJSON.Json.Deserialize(responseJsonData) as Dictionary<string, object>;

            // if we don't have an authResponse that means the player
            // hit cancel
            if (loginStatus["authResponse"] == null)
            {
                OnAuthResponse(new LoginResult(responseJsonData));
                return;
            }

            var authResponse = loginStatus["authResponse"] as Dictionary<string, object>;
            AccessToken token;
            if (!string.IsNullOrEmpty(authResponse["accessToken"] as string))
            {
                token = getAccessTokenFromAuthResponse(authResponse);
            } else
            {
                token = null;
            }

            // Call all our callbacks.
            OnAuthResponse(new LoginResult(responseJsonData, token));
        }

        public override void OnInitComplete(string responseJsonData)
        {
            OnFacebookAuthResponse(responseJsonData);
            if (this.onInitComplete != null)
            {
                this.onInitComplete();
            }
        }

        // TODO: Standarize with Android and iOS on complete handlers
        public override void OnLoginComplete(string responseJsonData)
        {
            throw new NotImplementedException();
        }

        public override void OnGetDeepLinkComplete(string message)
        {
            // We should never get here on canvas. We store the deep link on this object
            // so no
            throw new NotImplementedException();
        }

        // used only to refresh the access token
        public void OnFacebookAuthResponseChange(string responseJsonData)
        {
            var loginStatus = MiniJSON.Json.Deserialize(responseJsonData) as Dictionary<string, object>;

            if (loginStatus["authResponse"] == null)
            {
                return;
            }

            var authResponse = loginStatus["authResponse"] as Dictionary<string, object>;
            AccessToken.CurrentAccessToken = getAccessTokenFromAuthResponse(authResponse);
        }

        public void OnPayComplete(string responseJsonData)
        {
            string formattedResponse = CanvasFacebook.FormatResult(responseJsonData);
            var result = new PayResult(formattedResponse);
            CallbackManager.OnFacebookResponse(result);
        }

        public override void OnAppRequestsComplete(string responseJsonData)
        {
            string formattedResponse = CanvasFacebook.FormatResult(responseJsonData);
            var result = new AppRequestResult(formattedResponse);
            CallbackManager.OnFacebookResponse(result);
        }

        public override void OnShareLinkComplete(string responseJsonData)
        {
            string formattedResponse = CanvasFacebook.FormatResult(responseJsonData);
            var result = new ShareResult(formattedResponse);
            CallbackManager.OnFacebookResponse(result);
        }

        public override void OnGroupCreateComplete(string responseJsonData)
        {
            string formattedResponse = CanvasFacebook.FormatResult(responseJsonData);
            var result = new GroupCreateResult(formattedResponse);
            CallbackManager.OnFacebookResponse(result);
        }

        public override void OnGroupJoinComplete(string responseJsonData)
        {
            string formattedResponse = CanvasFacebook.FormatResult(responseJsonData);
            var result = new GroupJoinResult(formattedResponse);
            CallbackManager.OnFacebookResponse(result);
        }

        public void OnUrlResponse(string url)
        {
            deepLink = url;
        }

        public void OnHideUnity(bool hide)
        {
            if (this.OnHideUnityDelegate != null)
            {
                this.OnHideUnityDelegate(hide);
            }
        }

        #endregion

        public static AccessToken getAccessTokenFromAuthResponse(
            IDictionary<string, object> authResponse)
        {
            string accessToken = authResponse["accessToken"] as string;
            DateTime accessTokenExpiresAt =
                DateTime.Now.AddSeconds((Int64)authResponse["expiresIn"]);
            // empty string is a "Start Now" user
            string userId =
                CanvasFacebook.StringFromDictionary(authResponse, "userID");
            string permissionStr =
                CanvasFacebook.StringFromDictionary(authResponse, "grantedScopes");
            string[] permissions =
                string.IsNullOrEmpty(permissionStr) ? new string[0] : permissionStr.Split(',');
            return new AccessToken(accessToken, userId, accessTokenExpiresAt, permissions);
        }

        private static string StringFromDictionary(
            IDictionary<string, object> dictionary,
            string key)
        {
            object value;
            if (!dictionary.TryGetValue(key, out value) && value != null)
            {
                return null;
            }

            return value as string;
        }

        // This method converts the format of the result to match the format
        // of our results from iOS and Android
        private static string FormatResult(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                return result;
            }
            
            var resultDictionary = (IDictionary<string, object>)MiniJSON.Json.Deserialize(result);
            object response;
            if (resultDictionary.TryGetValue("response", out response))
            {
                string responseStr = response as string;
                if (responseStr != null) {
                    var responseDictionary = (IDictionary<string, object>)MiniJSON.Json.Deserialize(responseStr);
                    object callbackId;
                    if (resultDictionary.TryGetValue(Constants.CallbackIdKey, out callbackId))
                    {
                        responseDictionary[Constants.CallbackIdKey] = callbackId;
                    }
                    
                    return MiniJSON.Json.Serialize(responseDictionary);
                }
            }
            
            // No 'response' in string return origional string
            return result;
        }

        private class CanvasUIMethodCall<T> : MethodCall<T> where T : IResult
        {
            private CanvasFacebook canvasImpl;
            private string callbackMethod;

            public CanvasUIMethodCall(CanvasFacebook canvasImpl, string methodName, string callbackMethod)
                : base(canvasImpl, methodName)
            {
                this.canvasImpl = canvasImpl;
                this.callbackMethod = callbackMethod;
            }

            public override void call(MethodArguments args)
            {
                this.UI(this.MethodName, args, this.Callback);
            }

            private void UI(
                string method,
                MethodArguments args,
                FacebookDelegate<T> callback = null)
            {
                if (Screen.fullScreen)
                {
                    Screen.fullScreen = false;
                }

                var clonedArgs = new MethodArguments(args);
                clonedArgs.addNonNullOrEmptyParameter("app_id", this.canvasImpl.appId);
                clonedArgs.addNonNullOrEmptyParameter("method", method);
                var uniqueId = this.canvasImpl.CallbackManager.AddFacebookDelegate(callback);
                Application.ExternalCall("FBUnity.ui", clonedArgs.ToJsonString(), uniqueId, this.callbackMethod);
            }
        }
    }
}
