using System;

namespace Facebook.Unity
{
    internal interface ICanvasFacebookCallbackHandler : IFacebookCallbackHandler
    {
        void OnPayComplete(string message);

        // TODO: Standarize callbacks with mobile
        void OnFacebookAuthResponseChange(string message);
        void OnFacebookAuthResponse(string message);
        void OnUrlResponse(string message);
        void OnHideUnity(bool hide);
    }
}
