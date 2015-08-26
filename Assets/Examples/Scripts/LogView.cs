using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity.Example
{
    internal class LogView : ConsoleBase
    {
        private static IList<string> events = new List<string>();
        public static string datePatt = @"M/d/yyyy hh:mm:ss tt";

        void OnGUI()
        {
            GUILayout.BeginVertical();
            if (Button("Back"))
            {
                goBack();
            }

            #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                scrollPosition.y += Input.GetTouch(0).deltaPosition.y;
            }
            #endif
            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.MinWidth(mainWindowFullWidth));

            GUILayout.TextArea(
                string.Join("\n", events.ToArray()),
                textStyle,
                GUILayout.ExpandHeight(true),
                GUILayout.MaxWidth(mainWindowWidth));

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        public static void AddLog(string log)
        {
            events.Insert(0, String.Format("{0}\n{1}\n", DateTime.Now.ToString(datePatt), log));
        }
    }
}
