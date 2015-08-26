using System;
using UnityEngine;
using System.Collections;

namespace Facebook.Unity.Example
{
    internal class AppInvites : MenuBase
    {
        protected override void getGui()
        {
            if (Button("Android Invite"))
            {
                status = "Logged FB.AppEvent";
                FB.Mobile.AppInvite(new Uri("https://fb.me/892708710750483"), callback:handleResult);
            }

            if (Button("Android Invite With Custom Image"))
            {
                status = "Logged FB.AppEvent";
                FB.Mobile.AppInvite(new Uri("https://fb.me/892708710750483"), new Uri("http://i.imgur.com/zkYlB.jpg"), handleResult);
            }

            if (Button("iOS Invite"))
            {
                status = "Logged FB.AppEvent";
                FB.Mobile.AppInvite(new Uri("https://fb.me/810530068992919"), callback:handleResult);
            }

            if (Button("iOS Invite With Custom Image"))
            {
                status = "Logged FB.AppEvent";
                FB.Mobile.AppInvite(new Uri("https://fb.me/810530068992919"), new Uri("http://i.imgur.com/zkYlB.jpg"), handleResult);
            }
        }
    }
}
