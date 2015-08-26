using UnityEngine;
using System.Collections;

namespace Facebook.Unity
{
    internal class LoginResult : ResultBase, ILoginResult
    {
        public AccessToken AccessToken{ get; private set; }

        internal LoginResult(string response, AccessToken token = null) : base(response)
        {
            this.AccessToken = token;
        }
    }
}
