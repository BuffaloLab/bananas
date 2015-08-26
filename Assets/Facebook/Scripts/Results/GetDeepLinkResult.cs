using UnityEngine;
using System.Collections;

namespace Facebook.Unity
{
    internal class GetDeepLinkResult : ResultBase, IGetDeepLinkResult
    {
        public string DeepLink { get; private set; }

        internal GetDeepLinkResult(string result) : base(result)
        {
            if (this.ResultDictionary != null)
            {
                string deepLink;
                if (this.ResultDictionary.TryGetValue<string>("deep_link", out deepLink))
                {
                    this.DeepLink = deepLink;
                }
            }
        }
    }
}
