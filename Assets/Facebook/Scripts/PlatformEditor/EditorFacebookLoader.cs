using UnityEngine;
using System.Collections;

namespace Facebook.Unity.Editor
{
    internal class EditorFacebookLoader : FB.CompiledFacebookLoader
    {

        protected override FacebookGameObject fb
        {
            get
            {
                EditorFacebookGameObject editorFB = ComponentFactory.GetComponent<EditorFacebookGameObject>();
                editorFB.Facebook = new EditorFacebook();
                return editorFB;
            }
        }
    }
}
