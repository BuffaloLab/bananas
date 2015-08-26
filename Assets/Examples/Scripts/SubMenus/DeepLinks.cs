using UnityEngine;
using System.Collections;

namespace Facebook.Unity.Example
{
    internal class DeepLinks : MenuBase
    {
        protected override void getGui()
        {
            if (Button("Get Deep Link"))
            {
                CallFBGetDeepLink();
            }
        }

        private void CallFBGetDeepLink()
        {
            FB.GetDeepLink(handleResult);
        }
    }
}
