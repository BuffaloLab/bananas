using UnityEngine;
using System.Collections;

namespace Facebook.Unity
{
    internal class PayResult : ResultBase, IPayResult
    {
        internal PayResult(string result) : base(result) { }
    }
}
