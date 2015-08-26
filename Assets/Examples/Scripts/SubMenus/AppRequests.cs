using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity.Example
{
    internal class AppRequests : MenuBase
    {
        private string requestMessage = "";
        private string requestTo = "";
        private string requestFilter = "";
        private string requestExcludes = "";
        private string requestMax = "";
        private string requestData = "";
        private string requestTitle = "";
        private string requestObjectID = "";
        private int selectedAction = 0;
        private string[] actionTypeStrings = {
            "NONE",
            OGActionType.Send.ToString(),
            OGActionType.AskFor.ToString(),
            OGActionType.Turn.ToString()
        };


        private OGActionType getSelectedOGActionType()
        {
            string actionString = actionTypeStrings[selectedAction];
            if(actionString == OGActionType.Send.ToString())
                return OGActionType.Send;
            else if(actionString == OGActionType.AskFor.ToString())
                return OGActionType.AskFor;
            else if(actionString == OGActionType.Turn.ToString())
                return OGActionType.Turn;
            else
                return null;
        }

        protected override void getGui()
        {
            if (Button("Select - Filter None"))
            {
                FB.AppRequest("Test Message", callback: this.handleResult);
            }

            if (Button("Select - Filter app_users"))
            {
                List<object> filter = new List<object>(){"app_users"};
                // workaround for mono failing with named parameters
                FB.AppRequest("Test Message", null, filter, null, 0, "", "", this.handleResult);
            }

            if (Button("Select - Filter app_non_users"))
            {
                List<object> filter = new List<object>(){"app_non_users"};
                FB.AppRequest("Test Message", null, filter, null, 0, "", "", this.handleResult);
            }

            // Custom options
            LabelAndTextField("Message: ", ref requestMessage);
            LabelAndTextField("To (optional): ", ref requestTo);
            LabelAndTextField("Filter (optional): ", ref requestFilter);
            LabelAndTextField("Exclude Ids (optional): ", ref requestExcludes);
            LabelAndTextField("Filters: ", ref requestExcludes);
            LabelAndTextField("Max Recipients (optional): ", ref requestMax);
            LabelAndTextField("Data (optional): ", ref requestData);
            LabelAndTextField("Title (optional): ", ref requestTitle);

            GUILayout.BeginHorizontal();
            GUILayout.Label(
                "Request Action (optional): ",
                labelStyle, GUILayout.MaxWidth(200 * scaleFactor)
            );
            selectedAction = GUILayout.Toolbar(
                selectedAction,
                actionTypeStrings,
                buttonStyle,
                GUILayout.MinHeight(buttonHeight * scaleFactor),
                GUILayout.MaxWidth(mainWindowWidth - 150)
            );
            GUILayout.EndHorizontal();
            LabelAndTextField("Request Object ID (optional): ", ref requestObjectID);

            if (Button("Custom App Request"))
            {

                OGActionType action = getSelectedOGActionType();
                if(action != null)
                {
                    FB.AppRequest(
                        requestMessage,
                        action,
                        requestObjectID,
                        requestTo != null ? requestTo.Split(',') : null,
                        requestData,
                        requestTitle,
                        this.handleResult);
                }
                else
                {
                    FB.AppRequest(
                        requestMessage,
                        string.IsNullOrEmpty(requestTo) ? null : requestTo.Split(','),
                        string.IsNullOrEmpty(requestFilter) ? null : requestFilter.Split(',').OfType<object>().ToList(),
                        string.IsNullOrEmpty(requestExcludes) ? null: requestExcludes.Split(','),
                        string.IsNullOrEmpty(requestMax) ? 0 : int.Parse(requestMax),
                        requestData,
                        requestTitle,
                        this.handleResult);
                }
            }
        }
    }
}
