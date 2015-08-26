using UnityEngine;
using System.Collections;

namespace Facebook.Unity
{
    internal class ShareResult : ResultBase, IShareResult
    {
        public string PostId { get; private set; }

        internal ShareResult(string result) : base(result)
        {
            if (this.ResultDictionary != null) {
                object postId;
                if (this.ResultDictionary.TryGetValue("id", out postId))
                {
                    this.PostId = postId as string;
                }
            }
        }
    }
}
