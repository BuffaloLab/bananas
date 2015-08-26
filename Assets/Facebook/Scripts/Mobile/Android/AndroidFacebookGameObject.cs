using System;
using UnityEngine;

namespace Facebook.Unity.Mobile.Android
{
    internal class AndroidFacebookGameObject : MobileFacebookGameObject
    {
        protected override void OnAwake()
        {
#if UNITY_ANDROID
            AndroidJNIHelper.debug = true;
#endif
        }
    }
}
