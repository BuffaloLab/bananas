using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.Globalization;
using Facebook.Unity;

namespace UnityEditor.FacebookEditor
{
    public class ManifestMod
    {
        public const string DeepLinkingActivityName = "com.facebook.unity.FBUnityDeepLinkingActivity";
        public const string LoginActivityName = "com.facebook.LoginActivity";
        public const string UnityLoginActivityName = "com.facebook.unity.FBUnityLoginActivity";
        public const string UnityDialogsActivityName = "com.facebook.unity.FBUnityDialogsActivity";
        public const string UnityGameRequestActivityName = "com.facebook.unity.FBUnityGameRequestActivity";
        public const string UnityGameGroupCreateActivityName = "com.facebook.unity.FBUnityCreateGameGroupActivity";
        public const string UnityGameGroupJoinActivityName = "com.facebook.unity.FBUnityJoinGameGroupActivity";
        public const string UnityAppInviteDialogActivityName = "com.facebook.unity.AppInviteDialogActivity";
        public const string ApplicationIdMetaDataName = "com.facebook.sdk.ApplicationId";
        public const string FacebookContentProviderName = "com.facebook.FacebookContentProvider";
        public const string FacebookContentProviderAuthFormat = "com.facebook.app.FacebookContentProvider{0}";
        public const string FacebookActivityName = "com.facebook.FacebookActivity";

        public static void GenerateManifest()
        {
            var outputFile = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");

            // only copy over a fresh copy of the AndroidManifest if one does not exist
            if (!File.Exists(outputFile))
            {
                var inputFile = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines/androidplayer/AndroidManifest.xml");
                File.Copy(inputFile, outputFile);
            }
            UpdateManifest(outputFile);
        }

        public static bool CheckManifest()
        {
            bool result = true;
            var outputFile = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");
            if (!File.Exists(outputFile))
            {
                Debug.LogError("An android manifest must be generated for the Facebook SDK to work.  Go to Facebook->Edit Settings and press \"Regenerate Android Manifest\"");
                return false;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(outputFile);

            if (doc == null)
            {
                Debug.LogError("Couldn't load " + outputFile);
                return false;
            }

            XmlNode manNode = FindChildNode(doc, "manifest");
            XmlNode dict = FindChildNode(manNode, "application");

            if (dict == null)
            {
                Debug.LogError("Error parsing " + outputFile);
                return false;
            }

            string ns = dict.GetNamespaceOfPrefix("android");

            XmlElement loginElement = FindElementWithAndroidName("activity", "name", ns, UnityLoginActivityName, dict);
            if (loginElement == null)
            {
                Debug.LogError(string.Format("{0} is missing from your android manifest.  Go to Facebook->Edit Settings and press \"Regenerate Android Manifest\"", LoginActivityName));
                result = false;
            }

            var deprecatedMainActivityName = "com.facebook.unity.FBUnityPlayerActivity";
            XmlElement deprecatedElement = FindElementWithAndroidName("activity", "name", ns, deprecatedMainActivityName, dict);
            if (deprecatedElement != null)
            {
                Debug.LogWarning(string.Format("{0} is deprecated and no longer needed for the Facebook SDK.  Feel free to use your own main activity or use the default \"com.unity3d.player.UnityPlayerNativeActivity\"", deprecatedMainActivityName));
            }

            return result;
        }

        private static XmlNode FindChildNode(XmlNode parent, string name)
        {
            XmlNode curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name))
                {
                    return curr;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        private static XmlElement FindElementWithAndroidName(string name, string androidName, string ns, string value, XmlNode parent)
        {
            var curr = parent.FirstChild;
            while (curr != null)
            {
                if (curr.Name.Equals(name) && curr is XmlElement && ((XmlElement)curr).GetAttribute(androidName, ns) == value)
                {
                    return curr as XmlElement;
                }
                curr = curr.NextSibling;
            }
            return null;
        }

        public static void UpdateManifest(string fullPath)
        {
            string appId = FacebookSettings.AppId;

            if (!FacebookSettings.IsValidAppId)
            {
                Debug.LogError("You didn't specify a Facebook app ID.  Please add one using the Facebook menu in the main Unity editor.");
                return;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(fullPath);

            if (doc == null)
            {
                Debug.LogError("Couldn't load " + fullPath);
                return;
            }

            XmlNode manNode = FindChildNode(doc, "manifest");
            XmlNode dict = FindChildNode(manNode, "application");

            if (dict == null)
            {
                Debug.LogError("Error parsing " + fullPath);
                return;
            }

            string ns = dict.GetNamespaceOfPrefix("android");

            //add the unity login activity
            XmlElement unityLoginElement = FindElementWithAndroidName("activity", "name", ns, UnityLoginActivityName, dict);
            if (unityLoginElement == null)
            {
                unityLoginElement = CreateUnityOverlayElement(doc, ns, UnityLoginActivityName);
                dict.AppendChild(unityLoginElement);
            }

            //add the unity dialogs activity
            XmlElement unityDialogsElement = FindElementWithAndroidName("activity", "name", ns, UnityDialogsActivityName, dict);
            if (unityDialogsElement == null)
            {
                unityDialogsElement = CreateUnityOverlayElement(doc, ns, UnityDialogsActivityName);
                dict.AppendChild(unityDialogsElement);
            }

            //add the login activity
            XmlElement loginElement = FindElementWithAndroidName("activity", "name", ns, LoginActivityName, dict);
            if (loginElement == null)
            {
                loginElement = CreateLoginElement(doc, ns);
                dict.AppendChild(loginElement);
            }

            //add deep linking activity
            XmlElement deepLinkingElement = FindElementWithAndroidName("activity", "name", ns, DeepLinkingActivityName, dict);
            if (deepLinkingElement == null)
            {
                deepLinkingElement = CreateActivityElement(doc, ns, DeepLinkingActivityName);
                dict.AppendChild(deepLinkingElement);
            }

            //add game request activity
            XmlElement gameRequestElement = FindElementWithAndroidName("activity", "name", ns, UnityGameRequestActivityName, dict);
            if (gameRequestElement == null)
            {
                gameRequestElement = CreateActivityElement(doc, ns, UnityGameRequestActivityName);
                dict.AppendChild(gameRequestElement);
            }

            //add game group create activity
            XmlElement gameGroupCreateElement = FindElementWithAndroidName("activity", "name", ns, UnityGameGroupCreateActivityName, dict);
            if (gameGroupCreateElement == null)
            {
                gameGroupCreateElement = CreateActivityElement(doc, ns, UnityGameGroupCreateActivityName);
                dict.AppendChild(gameGroupCreateElement);
            }

            //add game group join activity
            XmlElement gameGroupJoinElement = FindElementWithAndroidName("activity", "name", ns, UnityGameGroupJoinActivityName, dict);
            if (gameGroupJoinElement == null)
            {
                gameGroupJoinElement = CreateActivityElement(doc, ns, UnityGameGroupJoinActivityName);
                dict.AppendChild(gameGroupJoinElement);
            }

            // add app invite activity
            XmlElement appInviteElement = FindElementWithAndroidName("activity", "name", ns, UnityAppInviteDialogActivityName, dict);
            if (appInviteElement == null)
            {
                appInviteElement = CreateActivityElement(doc, ns, UnityAppInviteDialogActivityName);
                dict.AppendChild(appInviteElement);
            }

            //add the app id
            //<meta-data android:name="com.facebook.sdk.ApplicationId" android:value="\ 409682555812308" />
            XmlElement appIdElement = FindElementWithAndroidName("meta-data", "name", ns, ApplicationIdMetaDataName, dict);
            if (appIdElement == null)
            {
                appIdElement = doc.CreateElement("meta-data");
                appIdElement.SetAttribute("name", ns, ApplicationIdMetaDataName);
                appIdElement.SetAttribute("value", ns, appId);
                dict.AppendChild(appIdElement);
            }

            // Add the facebook content provider
            // <provider
            //   android:name="com.facebook.FacebookContentProvider"
            //   android:authorities="com.facebook.app.FacebookContentProvider233936543368280"
            //   android:exported="true" />
            XmlElement contentProviderElement = FindElementWithAndroidName("provider", "name", ns, UnityGameRequestActivityName, dict);
            if (contentProviderElement == null)
            {
                contentProviderElement = CreateContentProviderElement(doc, ns, appId);
                dict.AppendChild(contentProviderElement);
            }

            // Add the facebook activity
            // <activity
            //   android:name="com.facebook.FacebookActivity"
            //   android:configChanges="keyboard|keyboardHidden|screenLayout|screenSize|orientation"
            //   android:label="@string/app_name"
            //   android:theme="@android:style/Theme.Translucent.NoTitleBar" />
            XmlElement facebookElement = FindElementWithAndroidName("activity", "name", ns, FacebookActivityName, dict);
            if (facebookElement == null)
            {
                facebookElement = CreateFacebookElement(doc, ns);
                dict.AppendChild(facebookElement);
            }

            doc.Save(fullPath);
        }

        private static XmlElement CreateLoginElement(XmlDocument doc, string ns)
        {
            //<activity android:name="com.facebook.LoginActivity" android:configChanges="keyboardHidden|orientation" android:theme="@android:style/Theme.Translucent.NoTitleBar.Fullscreen">
            //</activity>
            XmlElement activityElement = doc.CreateElement("activity");
            activityElement.SetAttribute("name", ns, LoginActivityName);
            activityElement.SetAttribute("configChanges", ns, "keyboardHidden|orientation");
            activityElement.SetAttribute("theme", ns, "@android:style/Theme.Translucent.NoTitleBar.Fullscreen");
            activityElement.InnerText = "\n    ";  //be extremely anal to make diff tools happy
            return activityElement;
        }

        private static XmlElement CreateUnityOverlayElement(XmlDocument doc, string ns, string activityName)
        {
            //<activity android:name="activityName" android:configChanges="all|of|them" android:theme="@android:style/Theme.Translucent.NoTitleBar.Fullscreen">
            //</activity>
            XmlElement activityElement = doc.CreateElement("activity");
            activityElement.SetAttribute("name", ns, activityName);
            activityElement.SetAttribute("configChanges", ns, "fontScale|keyboard|keyboardHidden|locale|mnc|mcc|navigation|orientation|screenLayout|screenSize|smallestScreenSize|uiMode|touchscreen");
            activityElement.SetAttribute("theme", ns, "@android:style/Theme.Translucent.NoTitleBar.Fullscreen");
            activityElement.InnerText = "\n    ";  //be extremely anal to make diff tools happy
            return activityElement;
        }

        private static XmlElement CreateFacebookElement(XmlDocument doc, string ns)
        {
            //<activity android:name="com.facebook.unity.FBUnityGameRequestActivity" android:exported="true">
            //</activity>
            XmlElement activityElement = doc.CreateElement("activity");
            activityElement.SetAttribute("name", ns, FacebookActivityName);
            activityElement.SetAttribute("configChanges", ns, "keyboard|keyboardHidden|screenLayout|screenSize|orientation");
            activityElement.SetAttribute("label", ns, "@string/app_name");
            activityElement.SetAttribute("theme", ns, "@android:style/Theme.Translucent.NoTitleBar");
            activityElement.InnerText = "\n    ";  //be extremely anal to make diff tools happy
            return activityElement;
        }

        private static XmlElement CreateContentProviderElement(XmlDocument doc, string ns, string appId)
        {
            //<activity android:name="com.facebook.unity.FBUnityGameRequestActivity" android:exported="true">
            //</activity>
            XmlElement activityElement = doc.CreateElement("activity");
            activityElement.SetAttribute("name", ns, FacebookContentProviderName);
            string authorities = string.Format(CultureInfo.InvariantCulture, FacebookContentProviderAuthFormat, appId);
            activityElement.SetAttribute ("authorities", ns, authorities);
            activityElement.SetAttribute("exported", ns, "true");
            activityElement.InnerText = "\n    ";  //be extremely anal to make diff tools happy
            return activityElement;
        }

        private static XmlElement CreateActivityElement(XmlDocument doc, string ns, string activityName)
        {
            //<activity android:name="activityName" android:exported="true">
            //</activity>
            XmlElement activityElement = doc.CreateElement("activity");
            activityElement.SetAttribute("name", ns, activityName);
            activityElement.SetAttribute("exported", ns, "true");
            activityElement.InnerText = "\n    ";  //be extremely anal to make diff tools happy
            return activityElement;
        }
    }
}
