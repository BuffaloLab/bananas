using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Facebook.Unity.Example
{
    internal class AppEvents : MenuBase
    {
        protected override void getGui()
        {
            if (Button("Log FB App Event"))
            {
                status = "Logged FB.AppEvent";
                FB.LogAppEvent(
                    AppEventName.UnlockedAchievement,
                    null,
                    new Dictionary<string,object>()
                    {
                    { AppEventParameterName.Description, "Clicked 'Log AppEvent' button" }
                }
                );
                LogView.AddLog(
                    "You may see results showing up at https://www.facebook.com/analytics/"
                    + FB.AppId);
            }
        }
    }
}
