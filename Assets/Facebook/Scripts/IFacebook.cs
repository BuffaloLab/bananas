using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facebook.Unity
{
    internal interface IFacebook
    {
        bool LoggedIn{ get; }
        bool LimitEventUsage { get; set; }
        string FacebookSdkVersion{ get; }

        void Init(
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

        void LogInWithPublishPermissions(
            string scope,
            FacebookDelegate<ILoginResult> callback);

        void LogInWithReadPermissions(
            string scope,
            FacebookDelegate<ILoginResult> callback);

        void LogOut();

        [Obsolete]
        void AppRequest(
            string message,
            string[] to,
            List<object> filters,
            string[] excludeIds,
            int? maxRecipients,
            string data,
            string title,
            FacebookDelegate<IAppRequestResult> callback);

        void AppRequest(
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

        void ShareLink(
            string contentURL,
            string contentTitle,
            string contentDescription,
            string photoURL,
            FacebookDelegate<IShareResult> callback);

        void FeedShare(
            string toId,
            Uri link,
            string linkName,
            string linkCaption,
            string linkDescription,
            Uri picture,
            string mediaSource,
            FacebookDelegate<IShareResult> callback);

        void GameGroupCreate(
            string name,
            string description,
            string privacy,
            FacebookDelegate<IGroupCreateResult> callback);

        void GameGroupJoin(
            string id,
            FacebookDelegate<IGroupJoinResult> callback);

        void API(
            string query,
            HttpMethod method,
            Dictionary<string, string> formData,
            FacebookDelegate<IGraphResult> callback);

        void API(
            string query,
            HttpMethod method,
            WWWForm formData,
            FacebookDelegate<IGraphResult> callback);

        void ActivateApp(string appId = null);

        void GetDeepLink(FacebookDelegate<IGetDeepLinkResult> callback);

        void AppEventsLogEvent(
            string logEvent,
            float? valueToSum,
            Dictionary<string, object> parameters);

        void AppEventsLogPurchase(
            float logPurchase,
            string currency,
            Dictionary<string, object> parameters);

    }
}
