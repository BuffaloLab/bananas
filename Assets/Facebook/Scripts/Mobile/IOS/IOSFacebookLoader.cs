using UnityEngine;
using System.Collections;

namespace Facebook.Unity.Mobile.IOS
{
    internal class IOSFacebookLoader : FB.CompiledFacebookLoader
    {

        protected override FacebookGameObject fb
        {
            get
            {
                IOSFacebookGameObject iosFB = ComponentFactory.GetComponent<IOSFacebookGameObject>();
                if (iosFB.Facebook == null)
                {
                    iosFB.Facebook = new IOSFacebook();
                }

                return iosFB;
            }
        }
    }
}
