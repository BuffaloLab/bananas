using System;

namespace Facebook.Unity.Mobile
{
    internal interface IMobileFacebookCallbackHandler : IFacebookCallbackHandler
    {
        void OnAppInviteComplete(string message);
    }
}
