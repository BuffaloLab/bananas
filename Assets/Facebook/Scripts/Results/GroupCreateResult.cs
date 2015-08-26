using UnityEngine;
using System.Collections;

namespace Facebook.Unity
{
    internal class GroupCreateResult : ResultBase, IGroupCreateResult
    {
        public string GroupId { get; private set; }

        internal GroupCreateResult(string result) : base (result)
        {
            if (this.ResultDictionary != null) {
                string groupId;
                if (this.ResultDictionary.TryGetValue<string>("id", out groupId))
                {
                    this.GroupId = groupId;
                }
            }
        }
    }
}
