using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Facebook.Unity
{
    internal class MethodArguments {
        private IDictionary<string, object> arguments = new Dictionary<string, object>();

        public MethodArguments() : this(new Dictionary<string,object>())
        {
        }

        public MethodArguments(MethodArguments methodArgs) : this(methodArgs.arguments)
        {
        }

        private MethodArguments(IDictionary<string, object> arguments)
        {
            this.arguments = arguments;
        }

        public void addNonNullOrEmptyParameter(string argumentName, int? nullable)
        {
            if (nullable != null && nullable.HasValue)
            {
                this.arguments[argumentName] = nullable.Value;
            }
        }

        public void addNonNullParameter(string argumentName, object value)
        {
            if (value != null)
            {
                this.arguments[argumentName] = value;
            }
        }

        public void addNonNullOrEmptyParameter(string argumentName, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                this.arguments[argumentName] = value;
            }
        }

        public void addCommaSeperateListNonNull(string argumentName, string[] value)
        {
            if (value != null)
            {
                this.arguments[argumentName] = string.Join(",", value);
            }
        }

        public void addNonNullOrEmptyParameter(string argumentName, Uri uri)
        {
            if (uri != null && !string.IsNullOrEmpty(uri.AbsoluteUri))
            {
                this.arguments[argumentName] = uri.ToString();
            }
        }

        public string ToJsonString()
        {
            return MiniJSON.Json.Serialize(arguments);
        }
    }
}
