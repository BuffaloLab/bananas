using UnityEngine;
using System.Collections;

namespace Facebook.Unity.Canvas
{
    internal class CanvasFacebookLoader : FB.CompiledFacebookLoader
    {
        protected override FacebookGameObject fb
        {
            get
            {
                CanvasFacebookGameObject canvasFB = ComponentFactory.GetComponent<CanvasFacebookGameObject>();
                if (canvasFB.Facebook == null)
                {
                    canvasFB.Facebook = new CanvasFacebook();
                }

                return canvasFB;
            }
        }
    }
}
