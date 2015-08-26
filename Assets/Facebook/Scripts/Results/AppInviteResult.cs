using UnityEngine;
using System.Collections;

namespace Facebook.Unity
{
    internal class AppInviteResult : ResultBase, IAppInviteResult
    {
        public AppInviteResult(string result) : base(result)
        {
        }
    }
}
