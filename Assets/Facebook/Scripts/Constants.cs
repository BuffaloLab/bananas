using UnityEngine;
using System.Collections;

namespace Facebook.Unity
{
    internal static class Constants {
        // Callback keys
        public const string CallbackIdKey = "callback_id";
        public const string AccessTokenKey = "access_token";
        public const string UserIdKey = "user_id";
        public const string ExpirationTimestampKey= "expiration_timestamp";
        public const string PermissionsKey = "permissions";

        // Callback Method Names
        public const string OnPayCompleteMethodName = "OnPayComplete";
        public const string OnShareCompleteMethodName = "OnShareLinkComplete";
        public const string OnAppRequestsCompleteMethodName = "OnAppRequestsComplete";
        public const string OnGroupCreateCompleteMethodName = "OnGroupCreateComplete";
        public const string OnJoinGroupCompleteMethodName = "OnJoinGroupComplete";

        // Graph API
        public const string GraphUrl = "https://graph.facebook.com";

        // Permission Strings
        public const string UserLikesPermission = "user_likes";
        public const string EmailPermission = "email";
        public const string PublishActionsPermission = "publish_actions";
        public const string PublishPagesPermission = "publish_pages";
    }
}
