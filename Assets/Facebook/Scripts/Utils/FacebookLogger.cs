using UnityEngine;
using Facebook.Unity.Mobile.Android;

namespace Facebook.Unity
{
    internal static class FacebookLogger
    {
        private const string UnityAndroidTag = "Facebook.Unity.FBDebug";

        internal static IFacebookLogger Instance{ private get; set; }

        static FacebookLogger()
        {
            FacebookLogger.Instance = new CustomLogger();
        }

        // TODO: route the canvas ones to call our facebook logging stuff in javascript
        public static void Log(string msg)
        {
            FacebookLogger.Instance.Log(msg);
        }

        public static void Info(string msg)
        {
            FacebookLogger.Instance.Info(msg);
        }

        public static void Warn(string msg)
        {
            FacebookLogger.Instance.Warn(msg);
        }

        public static void Error(string msg)
        {
            FacebookLogger.Instance.Error(msg);
        }

        private class CustomLogger : IFacebookLogger
        {
            IFacebookLogger logger;
            public CustomLogger()
            {
#if UNITY_ANDROID
                this.logger = new AndroidLogger();
#elif UNITY_IOS
                this.logger = new IOSLogger();
#elif !UNITY_EDITOR
                this.logger = new CanvasLogger();
#else
                this.logger = new EditorLogger();
#endif
            }

            public void Log(string msg)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.Log(msg);
                    this.logger.Log(msg);
                }
            }

            public void Info(string msg)
            {
                Debug.Log(msg);
                this.logger.Info(msg);
            }

            public void Warn(string msg)
            {
                Debug.LogWarning(msg);
                this.logger.Warn(msg);
            }

            public void Error(string msg)
            {
                Debug.LogError(msg);
                this.logger.Error(msg);
            }
        }

#if UNITY_ANDROID
        private class AndroidLogger : IFacebookLogger
        {
            public void Log(string msg)
            {
                using (AndroidJavaClass androidLogger = new AndroidJavaClass("android.util.Log"))
                {
                    androidLogger.CallStatic<int>("v", UnityAndroidTag, msg);
                }
            }

            public void Info(string msg)
            {
                using (AndroidJavaClass androidLogger = new AndroidJavaClass("android.util.Log"))
                {
                    androidLogger.CallStatic<int>("i", UnityAndroidTag, msg);
                }
            }

            public void Warn(string msg)
            {
                using (AndroidJavaClass androidLogger = new AndroidJavaClass("android.util.Log"))
                {
                    androidLogger.CallStatic<int>("w", UnityAndroidTag, msg);
                }
            }

            public void Error(string msg)
            {
                using (AndroidJavaClass androidLogger = new AndroidJavaClass("android.util.Log"))
                {
                    androidLogger.CallStatic<int>("e", UnityAndroidTag, msg);
                }
            }
        }
#elif UNITY_IOS
        private class IOSLogger: IFacebookLogger
        {
            public void Log(string msg)
            {
                // TODO
            }

            public void Info(string msg)
            {
                // TODO
            }

            public void Warn(string msg)
            {
                // TODO
            }

            public void Error(string msg)
            {
                // TODO
            }
        }
#elif !UNITY_EDITOR
        private class CanvasLogger : IFacebookLogger
        {
            public void Log(string msg)
            {
                Application.ExternalCall("console.log", msg);
            }

            public void Info(string msg)
            {
                Application.ExternalCall("console.info", msg);
            }

            public void Warn(string msg)
            {
                Application.ExternalCall("console.warn", msg);
            }

            public void Error(string msg)
            {
                Application.ExternalCall("console.error", msg);
            }
        }
#else
        private class EditorLogger : IFacebookLogger
        {
            public void Log(string msg)
            {
                // TODO
            }

            public void Info(string msg)
            {
                // TODO
            }

            public void Warn(string msg)
            {
                // TODO
            }

            public void Error(string msg)
            {
                // TODO
            }
        }
#endif
    }
}
