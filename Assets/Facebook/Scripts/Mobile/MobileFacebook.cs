// Copyright (c) 2014-present, Facebook, Inc. All rights reserved.
//
// You are hereby granted a non-exclusive, worldwide, royalty-free license to use,
// copy, modify, and distribute this software in source code or binary form for use
// in connection with the web services and APIs provided by Facebook.
//
// As with any software that integrates with the Facebook platform, your use of
// this software is subject to the Facebook Developer Principles and Policies
// [http://developers.facebook.com/policy/]. This copyright notice shall be
// included in all copies or substantial portions of the software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity.Mobile
{
    /// <summary>
    /// Classes defined on the mobile sdks
    /// </summary>
    internal abstract class MobileFacebook : FacebookBase, IMobileFacebookImplementation
    {
        private const string CallbackIdKey = "callback_id";
        private ShareDialogMode shareDialogMode = ShareDialogMode.AUTOMATIC;

        // Delegates 
        protected InitDelegate onInitComplete;

        /// <summary>
        /// Gets or sets the dialog mode.
        /// </summary>
        /// <value>The dialog mode use for sharing, login, and other dialogs.</value>
        public ShareDialogMode ShareDialogMode
        {
            get {
                return shareDialogMode;
            }

            set {
                shareDialogMode = value;
                this.setShareDialogMode(shareDialogMode);
            }
        }

        protected MobileFacebook(CallbackManager callbackManager) : base(callbackManager)
        {
        }


        public abstract void AppInvite(
            Uri appLinkUrl,
            Uri previewImageUrl,
            FacebookDelegate<IAppInviteResult> callback);

        public override void OnLoginComplete(string message)
        {
            LoginResult result;
            var parameters = (Dictionary<string, object>)MiniJSON.Json.Deserialize(message);
            if (parameters.ContainsKey("user_id"))
            {
                string userId = (string)parameters["user_id"];
                string accessToken = (string)parameters["access_token"];

                int expiredTimeSeconds;
                DateTime accessTokenExpiresAt;
                // If the date time is very large or 0 assume the token never expires
                if (int.TryParse((string)parameters["expiration_timestamp"], out expiredTimeSeconds) && expiredTimeSeconds > 0)
                {
                    accessTokenExpiresAt = FromTimestamp(expiredTimeSeconds);
                }
                else
                {
                    accessTokenExpiresAt = DateTime.MaxValue;
                }

                // Permissions can be array or string
                ICollection<string> permissions;
                FacebookLogger.Log(parameters["permissions"].GetType().FullName);
                string permissionStr = parameters["permissions"] as String;
                if (permissionStr != null)
                {
                    permissions = permissionStr.Split(',');
                } 
                else 
                {
                    // Assume we have an list
                    var rawPermissions = (IEnumerable<object>) parameters["permissions"];
                    permissions = rawPermissions.Select(permission => permission.ToString()).ToList();
                }

                AccessToken token = new AccessToken(
                    accessToken,
                    userId,
                    accessTokenExpiresAt,
                    permissions);
                result = new LoginResult(message, token);
            } else {
                result = new LoginResult(message);
            }

            OnAuthResponse(result);
        }

        public override void OnGetDeepLinkComplete(string message)
        {
            var result = new GetDeepLinkResult(message);
            CallbackManager.OnFacebookResponse(result);
        }

        public override void OnGroupCreateComplete(string message)
        {
            var result = new GroupCreateResult(message);
            CallbackManager.OnFacebookResponse(result);
        }

        public override void OnGroupJoinComplete(string message)
        {
            var result = new GroupJoinResult(message);
            CallbackManager.OnFacebookResponse(result);
        }

        public override void OnAppRequestsComplete(string message)
        {
            var result = new AppRequestResult(message);
            CallbackManager.OnFacebookResponse(result);
        }

        public void OnAppInviteComplete(string message)
        {
            var result = new AppInviteResult(message);
            CallbackManager.OnFacebookResponse(result);
        }

        public override void OnInitComplete(string message)
        {
            OnLoginComplete(message);
            if (this.onInitComplete != null)
            {
                this.onInitComplete();
            }
        }

        public override void OnShareLinkComplete(string message)
        {
            var result = new ShareResult(message);
            CallbackManager.OnFacebookResponse(result);
        }

        private static IDictionary<string, object> DeserializeMessage(string message)
        {
            return (Dictionary<string, object>)MiniJSON.Json.Deserialize(message);
        }

        private static string SerializeDictionary(IDictionary<string, object> dict)
        {
            return MiniJSON.Json.Serialize(dict);
        }

        private static bool TryGetCallbackId(IDictionary<string, object> result, out string callbackId)
        {
            object callback;
            callbackId = null;
            if (result.TryGetValue("callback_id", out callback))
            {
                callbackId = callback as string;
                return true;
            }
            
            return false;
        }

        private static bool TryGetError(IDictionary<string, object> result, out string errorMessage)
        {
            object error;
            errorMessage = null;
            if (result.TryGetValue("error", out error))
            {
                errorMessage = error as string;
                return true;
            }

            return false;
        }

        private static DateTime FromTimestamp(int timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);
        }

        protected static Dictionary<string, string> ToStringDict(Dictionary<string, object> dict)
        {
            if (dict == null)
            {
                return null;
            }
            var newDict = new Dictionary<string, string>();
            foreach (KeyValuePair<string, object> kvp in dict)
            {
                newDict [kvp.Key] = kvp.Value.ToString();
            }
            return newDict;
        }

        protected abstract void setShareDialogMode(ShareDialogMode mode);
    }
}
