using UnityEngine;

namespace Facebook.Unity
{
    internal class ComponentFactory
    {
        internal enum IfNotExist { AddNew, ReturnNull }

        public const string gameObjectName = "UnityFacebookSDKPlugin";

        private static GameObject facebookGameObject;

        private static GameObject FacebookGameObject
        {
            get
            {
                if (facebookGameObject == null)
                {
                    facebookGameObject = new GameObject(gameObjectName);
                }
                return facebookGameObject;
            }
        }

        /**
         * Gets one and only one component.  Lazy creates one if it doesn't exist
         */
        public static T GetComponent<T>(IfNotExist ifNotExist = IfNotExist.AddNew) where T : MonoBehaviour
        {
            var facebookGameObject = FacebookGameObject;

            T component = facebookGameObject.GetComponent<T>();
            if (component == null && ifNotExist == IfNotExist.AddNew)
            {
                component = facebookGameObject.AddComponent<T>();
            }

            return component;
        }

        /**
         * Creates a new component on the Facebook object regardless if there is already one
         */
        public static T AddComponent<T>() where T : MonoBehaviour
        {
            return FacebookGameObject.AddComponent<T>();
        }
    }
}
