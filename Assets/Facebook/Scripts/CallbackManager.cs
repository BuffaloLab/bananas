using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
    internal class CallbackManager
    {
        private IDictionary<string, object> facebookDelegates = new Dictionary<string, object>();
        private int nextAsyncId;

        public string AddFacebookDelegate<T>(FacebookDelegate<T> callback) where T: IResult
        {
            nextAsyncId++;
            facebookDelegates.Add(nextAsyncId.ToString(), callback);
            return nextAsyncId.ToString();
        }

        public void OnFacebookResponse(IInternalResult result)
        {
            FacebookLogger.Log(result.ToString());
            if (result == null || result.CallbackId == null)
            {
                return;
            }

            object callback;
            if (facebookDelegates.TryGetValue(result.CallbackId, out callback))
            {
                CallCallback(callback, result);
                facebookDelegates.Remove(result.CallbackId);
            }
        }

        // Since unity mono doesn't support covariance and contravariance use this hack
        private static void CallCallback(object callback, IResult result)
        {
            if (callback == null || result == null)
            {
                return;
            }

            if (CallbackManager.TryCallCallback<IAppRequestResult>(callback, result) ||
                CallbackManager.TryCallCallback<IShareResult>(callback, result) ||
                CallbackManager.TryCallCallback<IGroupCreateResult>(callback, result) ||
                CallbackManager.TryCallCallback<IGroupJoinResult>(callback, result) ||
                CallbackManager.TryCallCallback<IGetDeepLinkResult>(callback, result) ||
                CallbackManager.TryCallCallback<IPayResult>(callback, result) ||
                CallbackManager.TryCallCallback<IAppInviteResult>(callback, result))
            {
                return;
            }

            throw new NotSupportedException("Unexpected result type: " + callback.GetType().FullName);
        }

        private static bool TryCallCallback<T>(object callback, IResult result) where T : IResult
        {
            var castedCallback = callback as FacebookDelegate<T>;
            if (castedCallback != null)
            {
                castedCallback((T) result);
                return true;
            }
            return false;
        }
    }
}
