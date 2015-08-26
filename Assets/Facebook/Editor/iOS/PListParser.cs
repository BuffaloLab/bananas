using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.FacebookEditor
{
    public class PListParser
    {
        public PListDict xmlDict;
        private string filePath;

        public PListParser(string fullPath)
        {
            filePath = fullPath;
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ProhibitDtd = false;
            XmlReader plistReader = XmlReader.Create(filePath, settings);

            XDocument doc = XDocument.Load(plistReader);
            XElement plist = doc.Element("plist");
            XElement dict = plist.Element("dict");
            xmlDict = new PListDict(dict);
            plistReader.Close();
        }

        private void AddAppID(List<object> addToList, string appID, string urlSuffix)
        {
            string modifiedID = "fb" + appID;
            if(!string.IsNullOrEmpty(urlSuffix))
            {
                modifiedID += urlSuffix;
            }
            addToList.Add((object)modifiedID);
        }
        
        public void UpdateFBSettings(string appID, string urlSuffix)
        {
            xmlDict["FacebookAppID"] = appID;

            if (xmlDict.ContainsKey("CFBundleURLTypes"))
            {
                var currentSchemas = (List<object>)xmlDict["CFBundleURLTypes"];
                for (int i = 0; i < currentSchemas.Count; i++)
                {
                    // if it's not a dictionary, go to next index
                    if (currentSchemas[i].GetType() == typeof(PListDict))
                    {
                        var bundleTypeNode = (PListDict)currentSchemas[i];
                        if (bundleTypeNode.ContainsKey("CFBundleURLSchemes") && bundleTypeNode["CFBundleURLSchemes"].GetType() == typeof(List<object>))
                        {
                            var appIdsFromPListDict = (List<object>)bundleTypeNode["CFBundleURLSchemes"];
                            string firstAppID = (string)appIdsFromPListDict[0];
                            if (firstAppID.Contains("fb"))
                            {
                                // this is FB component
                                // clear old FB schemas, add current (editor properties) schemas
                                appIdsFromPListDict.Clear();
                                AddAppID(appIdsFromPListDict, appID, urlSuffix);
                                return;
                            }
                        }
                    }
                }

                // Didn't find FB schema, let's add FB schema to the list of schemas already present
                var appIds = new List<object>();
                AddAppID(appIds, appID, urlSuffix);
                var schemaEntry = new PListDict();
                schemaEntry.Add("CFBundleURLSchemes", appIds);
                currentSchemas.Add(schemaEntry);
                return;
            }
            else
            {
                // Didn't find any CFBundleURLTypes, let's create one
                var appIds = new List<object>();
                AddAppID(appIds, appID, urlSuffix);
                var schemaEntry = new PListDict();
                schemaEntry.Add("CFBundleURLSchemes",appIds);

                var currentSchemas = new List<object>();
                currentSchemas.Add(schemaEntry);
                xmlDict.Add("CFBundleURLTypes", currentSchemas);
            }
        }

        public void WriteToFile()
        {
            // Corrected header of the plist
            string publicId = "-//Apple//DTD PLIST 1.0//EN";
            string stringId = "http://www.apple.com/DTDs/PropertyList-1.0.dtd";
            string internalSubset = null;
            XDeclaration declaration = new XDeclaration("1.0", "UTF-8", null);
            XDocumentType docType = new XDocumentType("plist", publicId, stringId, internalSubset);

            xmlDict.Save(filePath, declaration, docType);
        }
   }
}
