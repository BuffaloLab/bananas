using System;

namespace Facebook.Unity.Mobile
{
    internal interface IMobileFacebook : IFacebook
    {
        ShareDialogMode ShareDialogMode { get; set; }

        void AppInvite(
            Uri appLinkUrl,
            Uri previewImageUrl,
            FacebookDelegate<IAppInviteResult> callback);
    }
}
