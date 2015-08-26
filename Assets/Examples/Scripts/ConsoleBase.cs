using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity.Example
{
    internal class ConsoleBase : MonoBehaviour
    {
        protected static Stack<string> menuStack = new Stack<string>();
        protected string status = "Ready";
        protected string lastResponse = "";
        protected Texture2D lastResponseTexture;
        protected Vector2 scrollPosition = Vector2.zero;
        #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
        protected int buttonHeight = 60;
        protected int mainWindowWidth = Screen.width - 30;
        protected int mainWindowFullWidth = Screen.width;
        #else
        protected int buttonHeight = 24;
        protected int mainWindowWidth = 500;
        protected int mainWindowFullWidth = 530;
        #endif

        // DPI scaling
        private const int DpiScalingFactor = 160;
        private float? _scaleFactor;
        private GUIStyle _textStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _textInputStyle;
        private GUIStyle _labelStyle;

        // Note we assume that these styles will be accessed from OnGUI otherwise the
        // unity APIs will fail.
        protected float scaleFactor
        {
            get
            {
                if (!_scaleFactor.HasValue)
                {
                    _scaleFactor = Screen.dpi / DpiScalingFactor;
                }

                return _scaleFactor.Value;
            }
        }

        protected int fontSize
        {
            get
            {
                return (int)Math.Round(scaleFactor * 16);
            }
        }

        protected GUIStyle textStyle
        {
            get
            {
                if (_textStyle == null)
                {
                    _textStyle = new GUIStyle(GUI.skin.textArea);
                    _textStyle.alignment = TextAnchor.UpperLeft;
                    _textStyle.wordWrap = true;
                    _textStyle.padding = new RectOffset(10, 10, 10, 10);
                    _textStyle.stretchHeight = true;
                    _textStyle.stretchWidth = false;
                    _textStyle.fontSize = fontSize;
                }

                return _textStyle;
            }
        }

        protected GUIStyle buttonStyle
        {
            get
            {
                if (_buttonStyle == null)
                {
                    _buttonStyle = new GUIStyle(GUI.skin.button);
                    _buttonStyle.fontSize = fontSize;
                }

                return _buttonStyle;
            }
        }

        protected GUIStyle textInputStyle
        {
            get
            {
                if (_textInputStyle == null)
                {
                    _textInputStyle = new GUIStyle(GUI.skin.textField);
                    _textInputStyle.fontSize = fontSize;
                }

                return _textInputStyle;
            }
        }

        protected GUIStyle labelStyle
        {
            get
            {
                if (_labelStyle == null)
                {
                    _labelStyle = new GUIStyle(GUI.skin.label);
                    _labelStyle.fontSize = fontSize;
                }

                return _labelStyle;
            }
        }

        virtual protected void Awake()
        {
            // Limit the framerate to 60 to keep device from burning through cpu
            Application.targetFrameRate = 60;
        }

        protected bool Button(string label)
        {
            return GUILayout.Button(
            label,
            buttonStyle,
            GUILayout.MinHeight(buttonHeight * scaleFactor),
            GUILayout.MaxWidth(mainWindowWidth)
            );
        }

        protected void LabelAndTextField(string label, ref string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, labelStyle, GUILayout.MaxWidth(200 * scaleFactor));
            text = GUILayout.TextField(
                text,
                textInputStyle,
                GUILayout.MaxWidth(mainWindowWidth - 150));
            GUILayout.EndHorizontal();
        }

        protected bool IsHorizontalLayout()
        {
            #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
            return Screen.orientation == ScreenOrientation.Landscape;
            #else
            return true;
            #endif
        }

        protected int TextWindowHeight
        {
            get
            {
                #if UNITY_IOS || UNITY_ANDROID || UNITY_WP8
                return IsHorizontalLayout() ? Screen.height : (int) Math.Round(85 * scaleFactor);
                #else
                return Screen.height;
                #endif
            }
        }

        protected void switchMenu(Type menuClass)
        {
            ConsoleBase.menuStack.Push(this.GetType().Name);
            Application.LoadLevel(menuClass.Name);
        }

        protected void goBack()
        {
            if (ConsoleBase.menuStack.Any())
            {
                Application.LoadLevel(ConsoleBase.menuStack.Pop());
            }
        }

    }
}
