using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Facebook.Unity {
    internal abstract class MethodCall<T> where T : IResult{

        protected FacebookBase facebookImpl;
        protected MethodArguments parameters = new MethodArguments();

        public string MethodName { get; private set; }

        public FacebookDelegate<T> Callback  { set; protected get; }
        
        public MethodCall(FacebookBase facebookImpl, string methodName)
        {
            this.facebookImpl = facebookImpl;
            this.MethodName = methodName;
        } 



        public abstract void call(MethodArguments args = null);
    }
}
