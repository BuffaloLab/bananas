//If you make changes in here make the same changes in Assets/Facebook/Editor/iOS/FBUnityInterface.h

namespace Facebook.Unity
{
    public enum ShareDialogMode
    {
        /// <summary>
        /// The sdk will choose which type of dialog to show
        /// See the Facebook SDKs for ios and android for specific details.
        /// </summary>
        AUTOMATIC = 0,
        /// <summary>
        /// Uses the dialog inside the native facebook applications. Note this will fail if the 
        /// native applications are not installed.
        /// </summary>
        NATIVE = 1,
        /// <summary>
        /// Opens the facebook dialog in a webview.
        /// </summary>
        WEB = 2,
        /// <summary>
        /// Uses the feed dialog.
        /// </summary>
        FEED = 3,
    };
}
