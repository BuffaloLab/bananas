using Facebook;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Facebook.Unity.Editor
{
    public class FacebookBuild
    {
        public enum Target { DEBUG, RELEASE }

        private const string FacebookPath = "Assets/Facebook/";
        private const string ExamplesPath = "Assets/Examples/";
        private const string PluginsPath = "Assets/Plugins/";
        
        // Exporting the *.unityPackage for Asset store
        public static void ExportPackage()
        {
            Debug.Log("Exporting Facebook Unity Package...");

            var path = "FacebookSDK.unitypackage";

            try
            {
                if (!File.Exists(Path.Combine(Application.dataPath, "Temp")))
                {
                    AssetDatabase.CreateFolder("Assets", "Temp");
                }
                AssetDatabase.MoveAsset(FacebookPath + "Resources/FacebookSettings.asset", "Assets/Temp/FacebookSettings.asset");
                AssetDatabase.DeleteAsset(PluginsPath + "Android/AndroidManifest.xml");
                AssetDatabase.DeleteAsset(PluginsPath + "Android/AndroidManifest.xml.meta");

                string[] facebookFiles = (string[])Directory.GetFiles(FacebookPath, "*.*", SearchOption.AllDirectories);
                string[] exampleFiles = (string[])Directory.GetFiles(ExamplesPath, "*.*", SearchOption.AllDirectories);
                string[] pluginsFiles = (string[])Directory.GetFiles(PluginsPath, "*.*", SearchOption.AllDirectories);
                string[] files = new string[facebookFiles.Length + exampleFiles.Length + pluginsFiles.Length];
                facebookFiles.CopyTo(files, 0);
                exampleFiles.CopyTo(files, facebookFiles.Length);
                pluginsFiles.CopyTo(files, facebookFiles.Length + exampleFiles.Length);

                AssetDatabase.ExportPackage(
                    files,
                    path,
                    ExportPackageOptions.IncludeDependencies | ExportPackageOptions.Recurse
                    );
            }
            finally
            {
                // Move files back no matter what
                AssetDatabase.MoveAsset("Assets/Temp/FacebookSettings.asset", FacebookPath + "Resources/FacebookSettings.asset");
                AssetDatabase.DeleteAsset("Assets/Temp");

                // regenerate the manifest
                UnityEditor.FacebookEditor.ManifestMod.GenerateManifest();
            }
            Debug.Log("Finished exporting!");
        }
    }
}
