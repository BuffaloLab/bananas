using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Facebook.Unity.Example
{
    internal class DialogShare : MenuBase
    {
        // Custom Share Link
        private string slLink = "https://developers.facebook.com/";
        private string slTitle = "Link Title";
        private string slDescription = "Link Description";
        private string slImage = "http://i.imgur.com/j4M7vCO.jpg";

        // Custom Feed Share
        private string fsTo = "";
        private string fsLink = "https://developers.facebook.com/";
        private string fsTitle = "Test Title";
        private string fsCaption = "Test Caption";
        private string fsDescription = "Test Description";
        private string fsImage = "http://i.imgur.com/zkYlB.jpg";
        private string fsMediaSource = "";

        protected override bool showDialogModeSelector()
        {
            #if UNITY_IOS || UNITY_ANDROID
            return true;
            #else
            return false;
            #endif
        }

        protected override void getGui()
        {
            if (Button("Share - Link"))
            {
                FB.ShareLink("https://developers.facebook.com/", callback: handleResult);
            }

            // Note: Web dialog doesn't support photo urls.
            if (Button("Share - Link Photo"))
            {
                FB.ShareLink(
                    "https://developers.facebook.com/",
                    "Link Share",
                    "Look I'm sharing a link",
                    "http://i.imgur.com/j4M7vCO.jpg",
                    callback: handleResult);
            }

            LabelAndTextField("Link", ref slLink);
            LabelAndTextField("Title", ref slTitle);
            LabelAndTextField("Description", ref slDescription);
            LabelAndTextField("Image", ref slImage);
            if (Button("Share - Custom"))
            {
                FB.ShareLink(
                    this.slLink,
                    this.slTitle,
                    this.slDescription,
                    this.slImage,
                    handleResult);
            }

            if (Button("Feed Share - No To"))
            {
                FB.FeedShare(
                    "",
                    new Uri("https://developers.facebook.com/"),
                    "Test Title",
                    "Test caption",
                    "Test Description",
                    new Uri("http://i.imgur.com/zkYlB.jpg"),
                    "",
                    handleResult);
            }

            LabelAndTextField("To", ref fsTo);
            LabelAndTextField("Link", ref fsLink);
            LabelAndTextField("Title", ref fsTitle);
            LabelAndTextField("Caption", ref fsCaption);
            LabelAndTextField("Description", ref fsDescription);
            LabelAndTextField("Image", ref fsImage);
            LabelAndTextField("Media Source", ref fsMediaSource);
            if (Button("Feed Share - Custom"))
            {
                FB.FeedShare(
                    this.fsTo,
                    string.IsNullOrEmpty(this.fsLink) ? null : new Uri(this.fsLink),
                    this.fsTitle,
                    this.fsCaption,
                    this.fsDescription,
                    string.IsNullOrEmpty(this.fsImage) ? null : new Uri(this.fsImage),
                    this.fsMediaSource,
                    handleResult);
            }
        }
    }
}
