using System;
using UnityEngine;

namespace Facebook.Unity.Mobile.Android
{
    internal class FBJavaClass : IAndroidJavaClass
    {
        private const string fbJavaClassName = "com.facebook.unity.FB";
        private AndroidJavaClass fbJavaClass = new AndroidJavaClass(fbJavaClassName);

        public T CallStatic<T>(string methodName)
        {
            return fbJavaClass.CallStatic<T>(methodName);
        }

        public void CallStatic(string methodName, params object[] args)
        {
            fbJavaClass.CallStatic(methodName, args);
        }

        // Mock the AndroidJava to compile on other platforms
        #if !UNITY_ANDROID
        private class AndroidJNIHelper{
            public static Boolean debug {get; set;}
        }

        private class AndroidJavaClass{
            public AndroidJavaClass(string mock) {}
            public T CallStatic<T>(string method) { return default(T); }
            public void CallStatic(string method, params object[] args) { }
        }
        #endif
    }
}
