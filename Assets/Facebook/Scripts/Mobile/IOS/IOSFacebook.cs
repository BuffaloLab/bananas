using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Facebook.Unity.Mobile.IOS
{
    internal class IOSFacebook : MobileFacebook
    {
        private const string CancelledResponse = "{\"cancelled\":true}";
        private bool limitEventUsage;
#if UNITY_IOS
        [DllImport ("__Internal")]
        private static extern void iosInit(
            string appId,
            bool cookie,
            bool logging,
            bool status,
            bool frictionlessRequests,
            string urlSuffix,
            string unityUserAgentSuffix);
        [DllImport ("__Internal")]
        private static extern void iosLogInWithReadPermissions(string scope);
        [DllImport ("__Internal")]
        private static extern void iosLogInWithPublishPermissions(string scope);
        [DllImport ("__Internal")]
        private static extern void iosLogOut();
        [DllImport ("__Internal")]
        private static extern void iosSetShareDialogMode(int mode);

        [DllImport ("__Internal")]
        private static extern void iosShareLink(
            int requestId,
            string contentURL,
            string contentTitle,
            string contentDescription,
            string photoURL);

        [DllImport ("__Internal")]
        public static extern void iosFeedShare(
            int requestId,
            string toId,
            string link,
            string linkName,
            string linkCaption,
            string linkDescription,
            string picture,
            string mediaSource);

        [DllImport ("__Internal")]
        private static extern void iosAppRequest(
            int requestId,
            string message,
            string actionType,
            string objectId,
            string[] to = null,
            int toLength = 0,
            string filters = "",
            string[] excludeIds = null,
            int excludeIdsLength = 0,
            bool hasMaxRecipients = false,
            int maxRecipients = 0,
            string data = "",
            string title = "");

        [DllImport ("__Internal")]
        private static extern void iosAppInvite(
            int requestId,
            string appLinkUrl,
            string previewImageUrl);

        [DllImport ("__Internal")]
        private static extern void iosCreateGameGroup(
            int requestId,
            string name,
            string description,
            string privacy);

        [DllImport ("__Internal")]
        private static extern void iosJoinGameGroup(int requestId, string groupId);

        [DllImport ("__Internal")]
        private static extern void iosFBSettingsActivateApp(string appId);

        [DllImport ("__Internal")]
        private static extern void iosFBAppEventsLogEvent(
            string logEvent,
            double valueToSum,
            int numParams,
            string[] paramKeys,
            string[] paramVals);

        [DllImport ("__Internal")]
        private static extern void iosFBAppEventsLogPurchase(
            double logPurchase,
            string currency,
            int numParams,
            string[] paramKeys,
            string[] paramVals);

        [DllImport ("__Internal")]
        private static extern void iosFBAppEventsSetLimitEventUsage(bool limitEventUsage);

        [DllImport ("__Internal")]
        private static extern void iosGetDeepLink(int requestID);

        [DllImport ("__Internal")]
        private static extern string iosFBSdkVersion();
#else
        void iosInit(string appId,
                     bool cookie,
                     bool logging,
                     bool status,
                     bool frictionlessRequests,
                     string urlSuffix,
                     string unityUserAgentSuffix) { }
        void iosLogInWithReadPermissions(string scope) { }
        void iosLogInWithPublishPermissions(string scope) { }
        void iosLogOut() { }

        void iosSetShareDialogMode(int mode) { }

        void iosShareLink(
            int requestId,
            string contentURL,
            string contentTitle,
            string contentDescription,
            string photoURL) { }

        void iosFeedShare(
            int requestId,
            string toId,
            string link,
            string linkName,
            string linkCaption,
            string linkDescription,
            string picture,
            string mediaSource) { }

        void iosAppRequest(
            int requestId,
            string message,
            string actionType,
            string objectId,
            string[] to = null,
            int toLength = 0,
            string filters = "",
            string[] excludeIds = null,
            int excludeIdsLength = 0,
            bool hasMaxRecipients = false,
            int maxRecipients = 0,
            string data = "",
            string title = "") { }

        void iosAppInvite(
            int requestId,
            string appLinkUrl,
            string previewImageUrl) { }

        void iosCreateGameGroup(
            int requestId,
            string name,
            string description,
            string privacy) { }

        void iosJoinGameGroup(int requestId, string groupId) {}

        void iosFBSettingsPublishInstall(int requestId, string appId) { }

        void iosFBSettingsActivateApp(string appId) { }

        void iosFBAppEventsLogEvent(
            string logEvent,
            double valueToSum,
            int numParams,
            string[] paramKeys,
            string[] paramVals) { }

        void iosFBAppEventsLogPurchase(
            double logPurchase,
            string currency,
            int numParams,
            string[] paramKeys,
            string[] paramVals) { }

        void iosFBAppEventsSetLimitEventUsage(bool limitEventUsage) { }

        void iosGetDeepLink(int requestId) { }

        string iosFBSdkVersion() {
            return "NONE";
        }
#endif

        private class NativeDict
        {
            public NativeDict()
            {
                numEntries = 0;
                keys = null;
                vals = null;
            }

            public int numEntries;
            public string[] keys;
            public string[] vals;
        };

        public enum FBInsightsFlushBehavior
        {
            FBInsightsFlushBehaviorAuto,
            FBInsightsFlushBehaviorExplicitOnly,
        };

        public override bool LimitEventUsage
        {
            get
            {
                return limitEventUsage;
            }
            set
            {
                limitEventUsage = value;
                iosFBAppEventsSetLimitEventUsage(value);
            }
        }

        public override string FacebookSdkVersion
        {
            get
            {
                return String.Format("Facebook.iOS.SDK.{0}", iosFBSdkVersion());
            }
        }

        public IOSFacebook()
            : this(new CallbackManager())
        {
        }

        public IOSFacebook(CallbackManager callbackManager)
            : base(callbackManager)
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
            string unityUserAgentSuffix = String.Format("Unity.{0}",
                                                        Facebook.Unity.FacebookSdkVersion.Build);
            iosInit(appId,
                    cookie,
                    logging,
                    status,
                    frictionlessRequests,
                    FacebookSettings.IosURLSuffix,
                    unityUserAgentSuffix);
            this.onInitComplete = onInitComplete;
        }

        #region FB public interface
        public override void LogInWithReadPermissions(
            string scope,
            FacebookDelegate<ILoginResult> callback)
        {
            AddAuthDelegate(callback);
            iosLogInWithReadPermissions(scope);
        }

        public override void LogInWithPublishPermissions(
            string scope,
            FacebookDelegate<ILoginResult> callback)
        {
            AddAuthDelegate(callback);
            iosLogInWithPublishPermissions(scope);
        }

        public override void LogOut()
        {
            base.LogOut();
            iosLogOut();
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

            string mobileFilter = null;
            if(filters != null && filters.Count > 0) {
                mobileFilter = filters[0] as string;
            }

            iosAppRequest(
                this.AddCallback(callback),
                message,
                (actionType != null) ? actionType.ToString() : "",
                objectId != null ? objectId : "",
                to,
                to != null ? to.Length : 0,
                mobileFilter != null ? mobileFilter : "",
                excludeIds,
                excludeIds != null ? excludeIds.Length : 0,
                maxRecipients.HasValue,
                maxRecipients.HasValue ? maxRecipients.Value : 0,
                data,
                title);
        }

        public override void AppInvite(
            Uri appLinkUrl,
            Uri previewImageUrl,
            FacebookDelegate<IAppInviteResult> callback)
        {
            string appLinkUrlStr = "";
            string previewImageUrlStr = "";
            if (appLinkUrl != null && !string.IsNullOrEmpty(appLinkUrl.AbsoluteUri))
            {
                appLinkUrlStr = appLinkUrl.AbsoluteUri;
            }

            if (previewImageUrl != null && !string.IsNullOrEmpty(previewImageUrl.AbsoluteUri))
            {
                previewImageUrlStr = previewImageUrl.AbsoluteUri;
            }

            iosAppInvite(
                this.AddCallback(callback),
                appLinkUrlStr,
                previewImageUrlStr);
        }

        public override void ShareLink(
            string contentURL,
            string contentTitle,
            string contentDescription,
            string photoURL,
            FacebookDelegate<IShareResult> callback)
        {
            iosShareLink(this.AddCallback(callback), contentURL, contentTitle, contentDescription, photoURL);
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
            string linkStr = link != null ? link.ToString() : "";
            string pictureStr = picture != null ? picture.ToString() : "";
            iosFeedShare(
                this.AddCallback(callback),
                toId,
                linkStr,
                linkName,
                linkCaption,
                linkDescription,
                pictureStr,
                mediaSource);
        }

        public override void GameGroupCreate(
            string name,
            string description,
            string privacy,
            FacebookDelegate<IGroupCreateResult> callback)
        {
            iosCreateGameGroup(this.AddCallback(callback), name, description, privacy);
        }

        public override void GameGroupJoin(
            string id,
            FacebookDelegate<IGroupJoinResult> callback)
        {
            iosJoinGameGroup(System.Convert.ToInt32(CallbackManager.AddFacebookDelegate(callback)), id);
        }

        public override void GetDeepLink(
            FacebookDelegate<IGetDeepLinkResult> callback)
        {
            if (callback == null)
            {
                return;
            }

            iosGetDeepLink(System.Convert.ToInt32(CallbackManager.AddFacebookDelegate(callback)));
        }

        public override void AppEventsLogEvent(
            string logEvent,
            float? valueToSum,
            Dictionary<string, object> parameters)
        {
            NativeDict dict = MarshallDict(parameters);
            if (valueToSum.HasValue)
            {
                iosFBAppEventsLogEvent(logEvent, valueToSum.Value, dict.numEntries, dict.keys, dict.vals);
            }
            else
            {
                iosFBAppEventsLogEvent(logEvent, 0.0, dict.numEntries, dict.keys, dict.vals);
            }
        }

        public override void AppEventsLogPurchase(
            float logPurchase,
            string currency,
            Dictionary<string, object> parameters)
        {
            NativeDict dict = MarshallDict(parameters);
            if (string.IsNullOrEmpty(currency))
            {
                currency = "USD";
            }
            iosFBAppEventsLogPurchase(logPurchase, currency, dict.numEntries, dict.keys, dict.vals);
        }

        public override void ActivateApp(string appId)
        {
            iosFBSettingsActivateApp(appId);
        }
        #endregion

        protected override void setShareDialogMode(ShareDialogMode mode)
        {
            iosSetShareDialogMode((int) mode);
        }

        #region Interal stuff
        private static NativeDict MarshallDict(Dictionary<string, object> dict)
        {
            NativeDict res = new NativeDict();

            if (dict != null && dict.Count > 0)
            {
                res.keys = new string[dict.Count];
                res.vals = new string[dict.Count];
                res.numEntries = 0;
                foreach (KeyValuePair<string, object> kvp in dict)
                {
                    res.keys[res.numEntries] = kvp.Key;
                    res.vals[res.numEntries] = kvp.Value.ToString();
                    res.numEntries++;
                }
            }
            return res;
        }

        private static NativeDict MarshallDict(Dictionary<string, string> dict)
        {
            NativeDict res = new NativeDict();

            if (dict != null && dict.Count > 0)
            {
                res.keys = new string[dict.Count];
                res.vals = new string[dict.Count];
                res.numEntries = 0;
                foreach (KeyValuePair<string, string> kvp in dict)
                {
                    res.keys[res.numEntries] = kvp.Key;
                    res.vals[res.numEntries] = kvp.Value;
                    res.numEntries++;
                }
            }
            return res;
        }

        private int AddCallback<T>(FacebookDelegate<T> callback)  where T: IResult
        {
            string asyncId = this.CallbackManager.AddFacebookDelegate(callback);
            return Convert.ToInt32(asyncId);
        }
        #endregion
    }
}
