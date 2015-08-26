using Facebook;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity.Editor;
using Facebook.Unity.Mobile.Android;
using Facebook.Unity.Mobile.IOS;
using Facebook.Unity.Canvas;
using Facebook.Unity.Mobile;

namespace Facebook.Unity
{
    public sealed class FB : ScriptableObject
    {
        public static InitDelegate OnInitComplete;
        public static HideUnityDelegate OnHideUnity;

        private static FacebookGameObject facebook;
        private static string authResponse;
        private static bool isInitCalled = false;
        private static string appId;
        private static bool cookie;
        private static bool logging;
        private static bool status;
        private static bool xfbml;
        private static bool frictionlessRequests;

        static IFacebook FacebookImpl
        {
            get
            {
                if (facebook == null)
                {
                    throw new NullReferenceException("Facebook object is not yet loaded.  Did you call FB.Init()?");
                }
                return facebook.Facebook;
            }
        }

        public static string AppId
        {
            get
            {
                // appId might be different from FBSettings.AppId
                // if using the programmatic version of FB.Init()
                return appId;
            }
        }

        public static bool IsLoggedIn
        {
            get
            {
                return (facebook != null) && FacebookImpl.LoggedIn;
            }
        }

        public static bool IsInitialized
        {
            get
            {
                return (facebook != null) && facebook.Initialized;
            }
        }

        #region Init
        /// <summary>
        /// This is the preferred way to call FB.Init().    It will take the facebook app id specified in your "Facebook"
        /// => "Edit Settings" menu when it is called.
        /// </summary>
        /// <param name="onInitComplete">
        /// Delegate is called when FB.Init() finished initializing everything. By passing in a delegate you can find
        /// out when you can safely call the other methods.
        /// </param>
        /// <param name="onHideUnity">A delegate to invoke when unity is hidden.</param>
        /// <param name="authResponse">Auth response.</param>
        public static void Init(InitDelegate onInitComplete, HideUnityDelegate onHideUnity = null, string authResponse = null)
        {
            Init(
                onInitComplete,
                FacebookSettings.AppId,
                FacebookSettings.Cookie,
                FacebookSettings.Logging,
                FacebookSettings.Status,
                FacebookSettings.Xfbml,
                FacebookSettings.FrictionlessRequests,
                onHideUnity,
                authResponse);
        }

        /**
          * If you need a more programmatic way to set the facebook app id and other setting call this function.
          * Useful for a build pipeline that requires no human input.
          */
          public static void Init(
            InitDelegate onInitComplete,
            string appId,
            bool cookie = true,
            bool logging = true,
            bool status = true,
            bool xfbml = false,
            bool frictionlessRequests = true,
            HideUnityDelegate onHideUnity = null,
            string authResponse = null)
        {
            FB.appId = appId;
            FB.cookie = cookie;
            FB.logging = logging;
            FB.status = status;
            FB.xfbml = xfbml;
            FB.frictionlessRequests = frictionlessRequests;
            FB.authResponse = authResponse;
            FB.OnInitComplete = onInitComplete;
            FB.OnHideUnity = onHideUnity;

            if (!isInitCalled)
            {
                FB.LogVersion();

    #if UNITY_EDITOR
                ComponentFactory.GetComponent<EditorFacebookLoader>();
    #elif UNITY_WEBPLAYER || UNITY_WEBGL
                ComponentFactory.GetComponent<CanvasFacebookLoader>();
    #elif UNITY_IOS
                ComponentFactory.GetComponent<IOSFacebookLoader>();
    #elif UNITY_ANDROID
                ComponentFactory.GetComponent<AndroidFacebookLoader>();
    #else
                throw new NotImplementedException("Facebook API does not yet support this platform");
    #endif
                isInitCalled = true;
                return;
            }

            FacebookLogger.Warn("FB.Init() has already been called.  You only need to call this once and only once.");

            // Init again if possible just in case something bad actually happened.
            if (FacebookImpl != null)
            {
                OnDllLoaded();
            }
        }

        private static void OnDllLoaded()
        {
            FB.LogVersion();
            FacebookImpl.Init(
                OnInitComplete,
                appId,
                cookie,
                logging,
                status,
                xfbml,
                FacebookSettings.ChannelUrl,
                authResponse,
                frictionlessRequests,
                OnHideUnity
            );
        }
    #endregion

        public static void LogInWithPublishPermissions(
            string scope = "",
            FacebookDelegate<ILoginResult> callback = null)
        {
            FacebookImpl.LogInWithPublishPermissions(scope, callback);
        }

        public static void LogInWithReadPermissions(
            string scope = "",
            FacebookDelegate<ILoginResult> callback = null)
        {
            FacebookImpl.LogInWithReadPermissions(scope, callback);
        }

        public static void LogOut()
        {
            FacebookImpl.LogOut();
        }

        public static void AppRequest(
            string message,
            OGActionType actionType,
            string objectId,
            string[] to,
            string data = "",
            string title = "",
            FacebookDelegate<IAppRequestResult> callback = null)
        {
            FacebookImpl.AppRequest(message, actionType, objectId, to, null, null, null, data, title, callback);
        }

        public static void AppRequest(
            string message,
            OGActionType actionType,
            string objectId,
            List<object> filters = null,
            string[] excludeIds = null,
            int? maxRecipients = null,
            string data = "",
            string title = "",
            FacebookDelegate<IAppRequestResult> callback = null)
        {
            FacebookImpl.AppRequest(message, actionType, objectId, null, filters, excludeIds, maxRecipients, data, title, callback);
        }

        public static void AppRequest(
            string message,
            string[] to = null,
            List<object> filters = null,
            string[] excludeIds = null,
            int? maxRecipients = null,
            string data = "",
            string title = "",
            FacebookDelegate<IAppRequestResult>callback = null)
        {
            FacebookImpl.AppRequest(message, null, null, to, filters, excludeIds, maxRecipients, data, title, callback);
        }

        public static void ShareLink(
            string contentURL = "",
            string contentTitle = "",
            string contentDescription = "",
            string photoURL = "",
            FacebookDelegate<IShareResult> callback = null)
        {
            FacebookImpl.ShareLink(
                contentURL,
                contentTitle,
                contentDescription,
                photoURL,
                callback);
        }

        /// <summary>
        /// Legacy feed share. Only use this dialog if you need the legacy parameters otherwiese use
        /// <see cref="FB.ShareLink(System.String, System.String, System.String, System.String, Facebook.FacebookDelegate"/>.
        /// </summary>
        /// <param name="toId">
        ///     The ID of the profile that this story will be published to.
        ///     If this is unspecified, it defaults to the value of from.
        ///     The ID must be a friend who also uses your app.
        /// </param>
        /// <param name="link">The link attached to this post.</param>
        /// <param name="linkName">The name of the link attachment.</param>
        /// <param name="linkCaption">
        ///     The caption of the link (appears beneath the link name).
        ///     If not specified, this field is automatically populated
        ///     with the URL of the link.
        /// </param>
        /// <param name="linkDescription">
        ///     The description of the link (appears beneath the link caption).
        ///     If not specified, this field is automatically populated by information
        ///     scraped from the link, typically the title of the page.
        /// </param>
        /// <param name="picture">
        ///     The URL of a picture attached to this post.
        ///     The picture must be at least 200px by 200px.
        ///     See our documentation on sharing best practices for more information on sizes.
        /// </param>
        /// <param name="mediaSource">
        ///     The URL of a media file (either SWF or MP3) attached to this post.
        ///     If SWF, you must also specify picture to provide a thumbnail for the video.
        /// </param>
        /// <param name="callback">The callback to use upon completion.</param>
        public static void FeedShare(
            string toId = "",
            Uri link = null,
            string linkName = "",
            string linkCaption = "",
            string linkDescription = "",
            Uri picture= null,
            string mediaSource = "",
            FacebookDelegate<IShareResult> callback = null)
        {
            FacebookImpl.FeedShare(
                toId,
                link,
                linkName,
                linkCaption,
                linkDescription,
                picture,
                mediaSource,
                callback);
        }

        public static void API(
            string query,
            HttpMethod method,
            FacebookDelegate<IGraphResult> callback = null,
            Dictionary<string, string> formData = null)
        {
            FacebookImpl.API(query, method, formData, callback);
        }

        public static void API(
            string query,
            HttpMethod method,
            FacebookDelegate<IGraphResult> callback,
            WWWForm formData)
        {
            FacebookImpl.API(query, method, formData, callback);
        }

        public static void ActivateApp()
        {
            FacebookImpl.ActivateApp(AppId);
        }

        public static void GetDeepLink(
            FacebookDelegate<IGetDeepLinkResult> callback)
        {
            FacebookImpl.GetDeepLink(callback);
        }

        public static void GameGroupCreate(
            string name,
            string description,
            string privacy = "CLOSED",
            FacebookDelegate<IGroupCreateResult> callback = null)
        {
            FacebookImpl.GameGroupCreate(name, description, privacy, callback);
        }

        public static void GameGroupJoin(
            string id,
            FacebookDelegate<IGroupJoinResult> callback = null)
        {
            FacebookImpl.GameGroupJoin(id, callback);
        }

        // If the player has set the limitEventUsage flag to YES, your app will continue
        // to send this data to Facebook, but Facebook will not use the data to serve
        // targeted ads. Facebook may continue to use the information for other purposes,
        // including frequency capping, conversion events, estimating the number of unique
        // users, security and fraud detection, and debugging.

        public static bool LimitAppEventUsage
        {
            get
            {
                return (facebook != null) && facebook.Facebook.LimitEventUsage;
            }
            set
            {
                if (facebook != null)
                {
                    facebook.Facebook.LimitEventUsage = value;
                }
            }
        }

        public static void LogAppEvent(
            string logEvent,
            float? valueToSum = null,
            Dictionary<string, object> parameters = null)
        {
            FacebookImpl.AppEventsLogEvent(logEvent, valueToSum, parameters);
        }

        /// <summary>
        /// Logs the purchase.
        /// </summary>
        /// <param name="logPurchase">The amount of currency the user spent.</param>
        /// <param name="currency">The 3-letter ISO currency code.</param>
        /// <param name="parameters">
        /// Any parameters needed to describe the event.
        /// Elements included in this dictionary can't be null.
        /// </param>
        public static void LogPurchase(
            float logPurchase,
            string currency = "USD",
            Dictionary<string, object> parameters = null)
        {
            FacebookImpl.AppEventsLogPurchase(logPurchase, currency, parameters);
        }

        #region Canvas-Only Implemented Methods
        public sealed class Canvas
        {
            private static ICanvasFacebook CanvasFacebookImpl
            {
                get
                {
                    ICanvasFacebook impl = FacebookImpl as ICanvasFacebook;
                    if (impl == null)
                    {
                        throw new InvalidOperationException("Attempt to call Canvas interface on non canvas platform");
                    }
                    return impl;
                }
            }

            public static void Pay(
                string product,
                string action = "purchaseitem",
                int quantity = 1,
                int? quantityMin = null,
                int? quantityMax = null,
                string requestId = null,
                string pricepointId = null,
                string testCurrency = null,
                FacebookDelegate<IPayResult> callback = null)
            {
                CanvasFacebookImpl.Pay(
                    product,
                    action,
                    quantity,
                    quantityMin,
                    quantityMax,
                    requestId,
                    pricepointId,
                    testCurrency,
                    callback);
            }
        }
        #endregion

        /// <summary>
        /// A class containing the settings specific to the supported mobile platforms.
        /// </summary>
        public sealed class Mobile
        {
            private static IMobileFacebook MobileFacebookImpl
            {
                get
                {
                    IMobileFacebook impl = FacebookImpl as IMobileFacebook;
                    if (impl == null)
                    {
                        throw new InvalidOperationException("Attempt to call Mobile interface on non mobile platform");
                    }
                    return impl;
                }
            }

            /// <summary>
            /// Gets or sets the share dialog mode.
            /// </summary>
            /// <value>The share dialog mode.</value>
            public static ShareDialogMode ShareDialogMode
            {
                get
                {
                    return Mobile.MobileFacebookImpl.ShareDialogMode;
                }

                set
                {
                    Mobile.MobileFacebookImpl.ShareDialogMode = value;
                }
            }

            /// <summary>
            /// Show the app invite dialog.
            /// </summary>
            /// <param name="appLinkUrl">
            ///     App Link for what should be opened when the recipient clicks on the
            ///     install/play button on the app invite page.
            /// </param>
            /// <param name="previewImageUrl">A url to an image to be used in the invite.</param>
            /// <param name="callback">A callback for when the dialog completes</para>
            public static void AppInvite(
                Uri appLinkUrl, 
                Uri previewImageUrl = null, 
                FacebookDelegate<IAppInviteResult> callback = null)
            {
                MobileFacebookImpl.AppInvite(appLinkUrl, previewImageUrl, callback);
            }

        }

        #region Android-Only Implemented Methods
        public sealed class Android
        {
            public static string KeyHash
            {
                get
                {
                    var androidFacebook = FacebookImpl as AndroidFacebook;
                    return (androidFacebook != null) ? androidFacebook.KeyHash : "";
                }
            }
        }
        #endregion

        #region CompiledFacebookLoader
        internal abstract class CompiledFacebookLoader : MonoBehaviour
        {
            protected abstract FacebookGameObject fb { get; }

            void Start()
            {
                FB.facebook = fb;
                FB.OnDllLoaded();
                Destroy(this);
            }
        }
        #endregion

        private static void LogVersion()
        {
            // If we have initlized we can also get the underlying sdk version
            if (facebook != null)
            {
                FacebookLogger.Info(String.Format(
                    "Using Unity SDK v{0} with {1}",
                    FacebookSdkVersion.Build,
                    FB.FacebookImpl.FacebookSdkVersion));
            }
            else
            {
                FacebookLogger.Info(String.Format("Using Unity SDK v{0}", FacebookSdkVersion.Build));
            }
        }
    }
}
