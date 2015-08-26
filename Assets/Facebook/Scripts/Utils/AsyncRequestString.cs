using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Facebook.Unity
{
    /*
     * A short lived async request that loads a FBResult from a url endpoint
     */
    internal class AsyncRequestString : MonoBehaviour
    {
        protected string url;
        protected HttpMethod method;
        protected Dictionary<string, string> formData;
        protected WWWForm query;
        protected FacebookDelegate<IGraphResult> callback;

        internal static void Post(
            string url,
            Dictionary<string, string> formData = null,
            FacebookDelegate<IGraphResult> callback = null)
        {
            Request(url, HttpMethod.POST, formData, callback);
        }

        internal static void Get(
            string url,
            Dictionary<string, string> formData = null,
            FacebookDelegate<IGraphResult> callback = null)
        {
            Request(url, HttpMethod.GET, formData, callback);
        }

        internal static void Request(
            string url,
            HttpMethod method,
            WWWForm query = null,
            FacebookDelegate<IGraphResult> callback = null)
        {
            ComponentFactory.AddComponent<AsyncRequestString>()
                .SetUrl(url)
                .SetMethod(method)
                .SetQuery(query)
                .SetCallback(callback);
        }

        internal static void Request(
            string url,
            HttpMethod method,
            Dictionary<string, string> formData = null,
            FacebookDelegate<IGraphResult> callback = null)
        {
            ComponentFactory.AddComponent<AsyncRequestString>()
                .SetUrl(url)
                .SetMethod(method)
                .SetFormData(formData)
                .SetCallback(callback);
        }

        IEnumerator Start()
        {
            WWW www;
            if (method == HttpMethod.GET)
            {
                string urlParams = (url.Contains("?")) ? "&" : "?";
                if (formData != null)
                {
                    foreach (KeyValuePair<string, string> pair in formData)
                    {
                        urlParams += string.Format("{0}={1}&", Uri.EscapeDataString(pair.Key), Uri.EscapeDataString(pair.Value));
                    }
                }
                www = new WWW(url + urlParams);
            }
            else //POST or DELETE
            {
                if (query == null)
                {
                    query = new WWWForm();
                }
                if (method == HttpMethod.DELETE)
                {
                    query.AddField("method", "delete");
                }
                if (formData != null)
                {
                    foreach (KeyValuePair<string, string> pair in formData)
                    {
                        query.AddField(pair.Key, pair.Value);
                    }
                }
                www = new WWW(url, query);
            }

            yield return www;

            if (callback != null)
            {
                callback(new GraphResult(www));
            }

            // after the callback is called, www should be able to be disposed
            www.Dispose();
            Destroy(this);
        }

        internal AsyncRequestString SetUrl(string url)
        {
            this.url = url;
            return this;
        }

        internal AsyncRequestString SetMethod(HttpMethod method)
        {
            this.method = method;
            return this;
        }

        internal AsyncRequestString SetFormData(Dictionary<string, string> formData)
        {
            this.formData = formData;
            return this;
        }

        internal AsyncRequestString SetQuery(WWWForm query)
        {
            this.query = query;
            return this;
        }

        internal AsyncRequestString SetCallback(FacebookDelegate<IGraphResult> callback)
        {
            this.callback = callback;
            return this;
        }

    }
}
