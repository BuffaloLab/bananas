using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Facebook.Unity
{
    internal class GraphResult : ResultBase, IGraphResult
    {
        public IList<object> ResultList {get; private set;}

        internal GraphResult(WWW result) : base(result.text, result.error, false)
        {
            init(this.RawResult);
        }

        private void init(string rawResult)
        {
            if (string.IsNullOrEmpty(rawResult))
            {
                return;
            }

            object serailizedResult = MiniJSON.Json.Deserialize(this.RawResult);
            var jsonObject = serailizedResult as IDictionary<string, object>;
            if (jsonObject != null)
            {
                this.ResultDictionary = jsonObject;
                return;
            }

            var jsonArray = serailizedResult as IList<object>;
            if (jsonArray != null)
            {
                this.ResultList = jsonArray;
                return;
            }
        }
    }
}
