using System;
using System.Collections.Generic;

namespace Facebook.Unity.Mobile.Android
{
    internal sealed class AndroidFacebook : MobileFacebook
    {
        // This class holds all the of the wrapper methods that we call into
        private bool limitEventUsage;
        private IAndroidJavaClass fbJava;

        // key Hash used for Android SDK
        public string KeyHash { get; private set; }

        public override bool LimitEventUsage
        {
            get
            {
                return limitEventUsage;
            }
            set
            {
                limitEventUsage = value;
                CallFB("SetLimitEventUsage", value.ToString());
            }
        }

        public override string FacebookSdkVersion
        {
            get
            {
                string buildVersion = this.fbJava.CallStatic<string>("GetSdkVersion");
                return String.Format("Facebook.Android.SDK.{0}", buildVersion);
            }
        }

        public AndroidFacebook() : this(new FBJavaClass(), new CallbackManager())
        {
        }

        public AndroidFacebook(IAndroidJavaClass fbJavaClass, CallbackManager callbackManager)
            : base(callbackManager)
        {
            this.KeyHash = "";
            this.fbJava = fbJavaClass;
        }

        private void CallFB(string method, string args)
        {
            this.fbJava.CallStatic(method, args);
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

            var args = new MethodArguments();
            args.addNonNullOrEmptyParameter("appId", appId);
            args.addNonNullParameter("cookie", cookie);
            args.addNonNullParameter("logging", logging);
            args.addNonNullParameter("status", status);
            args.addNonNullParameter("xfbml", xfbml);
            args.addNonNullOrEmptyParameter("channelUrl", channelUrl);
            args.addNonNullOrEmptyParameter("authResponse", authResponse);
            args.addNonNullParameter("frictionlessRequests", frictionlessRequests);
            var initCall = new JavaMethodCall<IResult>(this, "Init");
            initCall.call(args);
            this.CallFB("SetUserAgentSuffix",
                        String.Format("Unity.{0}", Facebook.Unity.FacebookSdkVersion.Build));
        }

        public override void LogInWithReadPermissions (
            string scope,
            FacebookDelegate<ILoginResult> callback)
        {
            MethodArguments args = new MethodArguments();
            args.addNonNullOrEmptyParameter("scope", scope);
            AddAuthDelegate (callback);
            var loginCall = new JavaMethodCall<IResult>(this, "LoginWithReadPermissions");
            loginCall.call(args);
        }

        public override void LogInWithPublishPermissions (
            string scope,
            FacebookDelegate<ILoginResult> callback)
        {
            MethodArguments args = new MethodArguments();
            args.addNonNullOrEmptyParameter("scope", scope);
            var loginCall = new JavaMethodCall<LoginResult>(this, "LoginWithPublishPermissions");
            AddAuthDelegate (callback);
            loginCall.call(args);
        }

        public override void LogOut()
        {
            var logoutCall = new JavaMethodCall<IResult>(this, "Logout");
            logoutCall.call();
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
            args.addNonNullOrEmptyParameter("action_type", actionType != null ? actionType.ToString() : null);
            args.addNonNullOrEmptyParameter("object_id", objectId);
            args.addCommaSeperateListNonNull("to", to);
            if (filters != null && filters.Count > 0)
            {
                string mobileFilter = filters[0] as string;
                if (mobileFilter != null)
                {
                    args.addNonNullOrEmptyParameter("filters", mobileFilter);
                }
            }
            args.addNonNullOrEmptyParameter("max_recipients", maxRecipients);
            args.addNonNullOrEmptyParameter("data", data);
            args.addNonNullOrEmptyParameter("title", title);
            var appRequestCall = new JavaMethodCall<IAppRequestResult>(this, "AppRequest");
            appRequestCall.Callback = callback;
            appRequestCall.call(args);
        }

        public override void AppInvite(
            Uri appLinkUrl,
            Uri previewImageUrl,
            FacebookDelegate<IAppInviteResult> callback)
        {
            var paramsDict = new Dictionary<string, object>();
            if (callback != null)
            {
                paramsDict["callback_id"] = CallbackManager.AddFacebookDelegate(callback);
            }

            if (appLinkUrl != null && !string.IsNullOrEmpty(appLinkUrl.AbsoluteUri))
            {
                paramsDict["appLinkUrl"] = appLinkUrl.AbsoluteUri;
            }

            if (previewImageUrl != null && !string.IsNullOrEmpty(previewImageUrl.AbsoluteUri))
            {
                paramsDict["previewImageUrl"] = previewImageUrl.AbsoluteUri;
            }

            CallFB("AppInvite", MiniJSON.Json.Serialize(paramsDict));
        }

        public override void ShareLink(
            string contentURL,
            string contentTitle,
            string contentDescription,
            string photoURL, 
            FacebookDelegate<IShareResult> callback)
        {
            MethodArguments args = new MethodArguments();
            args.addNonNullOrEmptyParameter("content_url", contentURL);
            args.addNonNullOrEmptyParameter("content_title", contentTitle);
            args.addNonNullOrEmptyParameter("content_description", contentDescription);
            args.addNonNullOrEmptyParameter("photo_url", photoURL);
            var shareLinkCall = new JavaMethodCall<IShareResult>(this, "ShareLink");
            shareLinkCall.Callback = callback;
            shareLinkCall.call(args);
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
            args.addNonNullOrEmptyParameter("toId", toId);
            args.addNonNullOrEmptyParameter("link", link);
            args.addNonNullOrEmptyParameter("linkName", linkName);
            args.addNonNullOrEmptyParameter("linkCaption", linkCaption);
            args.addNonNullOrEmptyParameter("linkDescription", linkDescription);
            args.addNonNullOrEmptyParameter("picture", picture);
            args.addNonNullOrEmptyParameter("mediaSource", mediaSource);
            var call = new JavaMethodCall<IShareResult>(this, "FeedShare");
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
            var gameGroupCreate = new JavaMethodCall<IGroupCreateResult>(this, "GameGroupCreate");
            gameGroupCreate.Callback = callback;
            gameGroupCreate.call(args);
        }

        public override void GameGroupJoin(
            string id,
            FacebookDelegate<IGroupJoinResult> callback)
        {
            var paramsDict = new Dictionary<string, object>();
            paramsDict["id"] = id;

            if (callback != null)
            {
                paramsDict["callback_id"] = CallbackManager.AddFacebookDelegate(callback);
            }

            CallFB("GameGroupJoin", MiniJSON.Json.Serialize (paramsDict));
        }

        public override void GetDeepLink(
            FacebookDelegate<IGetDeepLinkResult> callback)
        {
            if (callback != null)
            {
                var paramsDict = new Dictionary<string, object>();
                paramsDict["callback_id"] = CallbackManager.AddFacebookDelegate(callback);
                CallFB("GetDeepLink", MiniJSON.Json.Serialize (paramsDict));
            }
        }

        public override void AppEventsLogEvent(
            string logEvent,
            float? valueToSum,
            Dictionary<string, object> parameters)
        {
            var paramsDict = new Dictionary<string, object>();
            paramsDict["logEvent"] = logEvent;
            if (valueToSum.HasValue)
            {
                paramsDict["valueToSum"] = valueToSum.Value;
            }
            if (parameters != null)
            {
                paramsDict["parameters"] = ToStringDict(parameters);
            }
            CallFB("AppEvents", MiniJSON.Json.Serialize(paramsDict));
        }

        public override void AppEventsLogPurchase(
            float logPurchase,
            string currency,
            Dictionary<string, object> parameters)
        {
            var paramsDict = new Dictionary<string, object>();
            paramsDict["logPurchase"] = logPurchase;
            paramsDict["currency"] = (!string.IsNullOrEmpty(currency)) ? currency : "USD";
            if (parameters != null)
            {
                paramsDict["parameters"] = ToStringDict(parameters);
            }
            CallFB("AppEvents", MiniJSON.Json.Serialize(paramsDict));
        }

        public override void ActivateApp(string appId)
        {
            var parameters = new Dictionary<string, string>(1);
            if (!string.IsNullOrEmpty(appId))
            {
                parameters["app_id"] = appId;
            }
            CallFB("ActivateApp", MiniJSON.Json.Serialize(parameters));
        }

        protected override void setShareDialogMode(ShareDialogMode mode)
        {
            CallFB("SetShareDialogMode", mode.ToString());
        }

        private class JavaMethodCall<T> : MethodCall<T> where T : IResult
        {
            private AndroidFacebook androidImpl;

            public JavaMethodCall(AndroidFacebook androidImpl, string methodName)
                : base(androidImpl, methodName)
            {
                this.androidImpl = androidImpl;
            }

            public override void call(MethodArguments args = null)
            {
                MethodArguments paramsCopy;
                if (args == null)
                {
                    paramsCopy = new MethodArguments();
                } else {
                    paramsCopy = new MethodArguments(args);
                }

                if (this.Callback != null) {
                    paramsCopy.addNonNullParameter("callback_id", androidImpl.CallbackManager.AddFacebookDelegate(this.Callback));
                }
                androidImpl.CallFB(this.MethodName, paramsCopy.ToJsonString());
            }
        }
    }
}

