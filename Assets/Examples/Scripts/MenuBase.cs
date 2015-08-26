using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity.Example
{
    internal abstract class MenuBase : ConsoleBase
    {
        protected abstract void getGui();
        private static ShareDialogMode shareDialogMode;

        protected virtual bool showDialogModeSelector()
        {
            return false;
        }

        protected virtual bool showBackButton()
        {
            return true;
        }

        protected void handleResult(IResult result)
        {
            if (result == null)
            {
                lastResponse = "Null Response\n";
                LogView.AddLog(lastResponse);
                return;
            }

            lastResponseTexture = null;
            // Some platforms return the empty string instead of null.
            if (!String.IsNullOrEmpty(result.Error))
            {
                status = "Error - Check log for details";
                lastResponse = "Error Response:\n" + result.Error;
                LogView.AddLog(result.Error);
            } else if (!String.IsNullOrEmpty(result.RawResult))
            {
                status = "Success - Check log for details";
                lastResponse = "Success Response:\n" + result.RawResult;
                LogView.AddLog(result.RawResult);
            } else
            {
                lastResponse = "Empty Response\n";
                LogView.AddLog(lastResponse);
            }
        }

        void OnGUI()
        {
            if (IsHorizontalLayout())
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
            }

            GUILayout.Label(this.GetType().Name, labelStyle);

            addStatus();

            #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                scrollPosition.y += Input.GetTouch(0).deltaPosition.y;
            }
            #endif
            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.MinWidth(mainWindowFullWidth));

            GUILayout.BeginHorizontal();
            if (showBackButton())
            {
                addBackButton();
            }
            addLogButton();
            GUILayout.EndHorizontal();
            if (showDialogModeSelector())
            {
                addDialogModeButtons();
            }

            GUILayout.BeginVertical();
            // Add the ui from decendants
            getGui();
            GUILayout.Space(10);

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            addLastResponseTextArea();
        }

        private void addStatus()
        {
            GUILayout.Space(5);
            GUILayout.Box("Status: " + status, textStyle, GUILayout.MinWidth(mainWindowWidth));
        }

        private void addBackButton()
        {
            GUI.enabled = menuStack.Any();
            if (Button("Back"))
            {
                this.goBack();
            }
            GUI.enabled = true;
        }

        private void addLogButton()
        {
            if (Button("Log"))
            {
                switchMenu(typeof(LogView));
            }
        }

        private void addDialogModeButtons()
        {
            GUILayout.BeginHorizontal();
            foreach (var value in Enum.GetValues(
                typeof(ShareDialogMode))
                     .Cast<ShareDialogMode>())
            {
                addDialogModeButton(value);
            }
            GUILayout.EndHorizontal();
        }

        private void addDialogModeButton(ShareDialogMode mode)
        {
            bool enabled = GUI.enabled;
            GUI.enabled = enabled && (mode != shareDialogMode);
            if (Button(mode.ToString()))
            {
                shareDialogMode = mode;
                FB.Mobile.ShareDialogMode = mode;
            }
            GUI.enabled = enabled;
        }

        private void addLastResponseTextArea()
        {
            if (IsHorizontalLayout())
            {
                GUILayout.EndVertical();
            }

            var textAreaSize = GUILayoutUtility.GetRect(640, TextWindowHeight);

            GUI.TextArea(
                textAreaSize,
                string.Format(
                " AppId: {0} \n UserId: {1}\n IsLoggedIn: {2}\n {3}",
                FB.AppId,
                FB.IsLoggedIn ? AccessToken.CurrentAccessToken.UserId : "None",
                FB.IsLoggedIn,
                lastResponse
                ), textStyle);

            if (lastResponseTexture != null)
            {
                var texHeight = textAreaSize.y + 200;
                if (Screen.height - lastResponseTexture.height < texHeight)
                {
                    texHeight = Screen.height - lastResponseTexture.height;
                }
                GUI.Label(
                    new Rect(
                        textAreaSize.x + 5,
                        texHeight,
                        lastResponseTexture.width,
                        lastResponseTexture.height),
                    lastResponseTexture);
            }

            if (IsHorizontalLayout())
            {
                GUILayout.EndHorizontal();
            }
        }
    }
}
