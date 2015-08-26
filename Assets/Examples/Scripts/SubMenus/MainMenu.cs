using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity.Example
{
    internal sealed class MainMenu : MenuBase
    {
        protected override bool showBackButton()
        {
            return false;
        }

        protected override void getGui()
        {
            bool enabled = GUI.enabled;
            if (Button("FB.Init"))
            {
                CallFBInit();
                status = "FB.Init() called with " + FB.AppId;
            }

            GUILayout.BeginHorizontal();

            GUI.enabled = enabled && FB.IsInitialized;
            if (Button("Login"))
            {
                CallFBLogin();
                status = "Login called";
            }

            GUI.enabled = FB.IsLoggedIn;
            if (Button("Get publish_actions"))
            {
                CallFBLoginForPublish();
                status = "Login (for publish_actions) called";
            }

            #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_EDITOR
            if (Button("Logout"))
            {
                CallFBLogout();
                status = "Logout called";
            }
            #endif
            GUILayout.EndHorizontal();

            GUI.enabled = enabled && FB.IsInitialized;
            if (Button("Share Dialog"))
            {
                this.switchMenu(typeof(DialogShare));
            }

            if (Button("Game Groups"))
            {
                this.switchMenu(typeof(GameGroups));
            }

            if (Button("App Requests"))
            {
                this.switchMenu(typeof(AppRequests));
            }

            if (Button("Graph Request"))
            {
                this.switchMenu(typeof(GraphRequest));
            }

            #if UNITY_WEBPLAYER
            if (Button("Pay"))
            {
                this.switchMenu(typeof(Pay));
            }
            #endif

            if (Button("App Events"))
            {
                this.switchMenu(typeof(AppEvents));
            }

            if (Button("Deep Links"))
            {
                this.switchMenu(typeof(DeepLinks));
            }

            #if UNITY_IOS || UNITY_ANDROID
            if (Button("App Invites"))
            {
                this.switchMenu(typeof(AppInvites));
            }
            #endif
            GUI.enabled = enabled;
        }

        private void CallFBLogin()
        {
            FB.LogInWithReadPermissions("public_profile,email,user_friends", handleResult);
        }

        private void CallFBLoginForPublish()
        {
            // It is generally good behavior to split asking for read and publish
            // permissions rather than ask for them all at once.
            //
            // In your own game, consider postponing this call until the moment
            // you actually need it.
            FB.LogInWithPublishPermissions("publish_actions", handleResult);
        }

        private void CallFBLogout()
        {
            FB.LogOut();
        }

        private void CallFBInit()
        {
            FB.Init(OnInitComplete, OnHideUnity);
        }

        private void OnInitComplete()
        {
            Debug.Log("FB.Init completed.");
        }

        private void OnHideUnity(bool isGameShown)
        {
            Debug.Log("Is game showing? " + isGameShown);
        }
    }
}
