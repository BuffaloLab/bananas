using UnityEngine;
using System.Collections;

namespace Facebook.Unity
{
    internal class GroupJoinResult : ResultBase, IGroupJoinResult
    {
        internal GroupJoinResult(string result) : base(result) { }
    }
}
