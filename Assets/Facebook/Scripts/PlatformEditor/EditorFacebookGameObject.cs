using System;
using System.Collections.Generic;


namespace Facebook.Unity.Editor
{
    internal class EditorFacebookGameObject : FacebookGameObject
    {
        // Callback for getting the user id.
        public void MockLoginCallback(IGraphResult result)
        {
            Destroy(ComponentFactory.GetComponent<EditorFacebookAccessToken>());
            if (result.Error != null)
            {
                var errorStruct = (Dictionary<string, object>) MiniJSON.Json.Deserialize(result.RawResult);
                if(errorStruct.ContainsKey("error"))
                {
                    var errorMessage = errorStruct["error"] as Dictionary<string, object>;

                    var msg = errorMessage.ContainsKey("message") ? errorMessage["message"] : null;
                    var type = errorMessage.ContainsKey("type") ? errorMessage["type"] : null;
                    var code = errorMessage.ContainsKey("code") ? errorMessage["code"] : null;

                    BadAccessToken(type + ": " + code + " " + msg);
                    return;
                }

                BadAccessToken(result.Error);
                return;
            }

            try
            {
                var json = (List<object>) MiniJSON.Json.Deserialize(result.RawResult);
                var responses = new List<string>();
                foreach (object obj in json)
                {
                    if (!(obj is Dictionary<string, object>))
                    {
                        continue;
                    }

                    var response = (Dictionary<string, object>) obj;

                    if (!response.ContainsKey("body"))
                    {
                        continue;
                    }

                    responses.Add((string) response["body"]);
                }

                var userData = (Dictionary<string, object>) MiniJSON.Json.Deserialize(responses[0]);
                var appData = (Dictionary<string, object>) MiniJSON.Json.Deserialize(responses[1]);

                if (FB.AppId != (string) appData["id"])
                {
                    BadAccessToken("Access token is not for current app id: " + FB.AppId);
                    return;
                }

                string userId = (string)userData["id"];
                // The access token as already been set before this. Its just missing the user ID so we
                // Set it here
                AccessToken.CurrentAccessToken.UserId = userId;

            }
            catch (Exception e)
            {
                BadAccessToken("Could not get data from access token: " + e.Message);
            }
        }

        public void MockCancelledLoginCallback()
        {
            this.OnLoginComplete("");
        }

        private void BadAccessToken(string error)
        {
            FacebookLogger.Error(error);
            AccessToken.CurrentAccessToken = null;
            ComponentFactory.GetComponent<EditorFacebookAccessToken>();
        }
    }
}
