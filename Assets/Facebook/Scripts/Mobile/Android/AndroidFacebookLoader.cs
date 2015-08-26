using UnityEngine;
using System.Collections;

namespace Facebook.Unity.Mobile.Android
{
    internal class AndroidFacebookLoader : FB.CompiledFacebookLoader
    {

        protected override FacebookGameObject fb
        {
            get
            {
                AndroidFacebookGameObject androidFB = ComponentFactory.GetComponent<AndroidFacebookGameObject>();
                if (androidFB.Facebook == null)
                {
                    androidFB.Facebook = new AndroidFacebook();
                }

                return androidFB;
            }
        }
    }
}
