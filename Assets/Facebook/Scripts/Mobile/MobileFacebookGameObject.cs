using System;

namespace Facebook.Unity.Mobile
{
    internal abstract class MobileFacebookGameObject : FacebookGameObject, IMobileFacebookCallbackHandler
    {
        private IMobileFacebookImplementation MobileFacebook
        {
            get
            {
                return (IMobileFacebookImplementation) this.Facebook;
            }
        }

        public void OnAppInviteComplete(string message)
        {
            this.MobileFacebook.OnAppInviteComplete(message);
        }
    }
}
