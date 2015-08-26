using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Facebook.Unity
{
    internal class AppRequestResult : ResultBase, IAppRequestResult
    {
        public string RequestID { get; private set; }
        public IEnumerable<string> To { get; private set; }

        public AppRequestResult(string result) : base (result)
        {
            if (this.ResultDictionary != null) {
                string requestID;
                if (this.ResultDictionary.TryGetValue<string>("request", out requestID))
                {
                    this.RequestID = requestID;
                }

                string toStr;
                if (this.ResultDictionary.TryGetValue<string>("to", out toStr))
                {
                    this.To = toStr.Split(',');
                }
            }
        }
    }
}
