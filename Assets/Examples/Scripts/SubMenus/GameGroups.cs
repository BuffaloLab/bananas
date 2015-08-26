using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Facebook.Unity.Example
{
    internal class GameGroups : MenuBase
    {
        public string GamerGroupName = "Test group";
        public string GamerGroupDesc = "Test group for testing.";
        public string GamerGroupPrivacy = "closed";
        public string GamerGroupAdmin = "";
        public string GamerGroupCurrentGroup = "";

        private void GroupCreateCB(IGroupCreateResult result)
        {
            handleResult(result);
            if (result.GroupId != null)
            {
                GamerGroupCurrentGroup = result.GroupId;
            }
        }

        void GetAllGroupsCB(IGraphResult result)
        {
            if (!String.IsNullOrEmpty(result.RawResult))
            {
                lastResponse = result.RawResult;
                var resultDictionary = result.ResultDictionary;
                if (resultDictionary.ContainsKey("data"))
                {
                    var dataArray = (List<object>)resultDictionary["data"];

                    if (dataArray.Count > 0)
                    {
                        var firstGroup = (Dictionary<string, object>)dataArray [0];
                        GamerGroupCurrentGroup = (string)firstGroup ["id"];

                    }
                }
            }
            if (!String.IsNullOrEmpty(result.Error))
            {
                lastResponse = result.Error;
            }
        }

        private void CallFbGetAllOwnedGroups()
        {
            FB.API(FB.AppId + "/groups", HttpMethod.GET, GetAllGroupsCB);
        }

        private void CallFbGetUserGroups()
        {
            FB.API("/me/groups?parent=" + FB.AppId, HttpMethod.GET, handleResult);
        }

        private void CallCreateGroupDialog()
        {
            FB.GameGroupCreate(
                GamerGroupName,
                GamerGroupDesc,
                GamerGroupPrivacy,
                GroupCreateCB);
        }

        private void CallJoinGroupDialog()
        {
            FB.GameGroupJoin(
                GamerGroupCurrentGroup,
                handleResult);
        }

        private void CallFbPostToGamerGroup()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict ["message"] = "herp derp a post";

            FB.API(GamerGroupCurrentGroup + "/feed", HttpMethod.POST, handleResult, dict);
        }

        protected override void getGui()
        {
            if (Button("Game Group Create - Closed"))
            {
                FB.GameGroupCreate(
                    "Test game group",
                    "Test description",
                    "CLOSED",
                    this.handleResult);
            }

            if (Button("Game Group Create - Open"))
            {
                FB.GameGroupCreate(
                    "Test game group",
                    "Test description",
                    "OPEN",
                    this.handleResult);
            }

            LabelAndTextField("Group Name", ref GamerGroupName);
            LabelAndTextField("Group Description", ref GamerGroupDesc);
            LabelAndTextField("Group Privacy", ref GamerGroupPrivacy);

            if (Button("Call Create Group Dialog"))
            {
                CallCreateGroupDialog();
            }

            LabelAndTextField("Group To Join", ref GamerGroupCurrentGroup);
            if (Button("Call Join Group Dialog"))
            {
                CallJoinGroupDialog();
            }

            bool enabled = GUI.enabled;
            GUI.enabled = enabled && FB.IsLoggedIn;
            if (Button("Get All App Managed Groups"))
            {
                CallFbGetAllOwnedGroups();
            }

            if (Button("Get Gamer Groups Logged in User Belongs to"))
            {
                CallFbGetUserGroups();
            }

            if (Button("Make Group Post As User"))
            {
                CallFbPostToGamerGroup();
            }
            GUI.enabled = enabled;
        }
    }
}
