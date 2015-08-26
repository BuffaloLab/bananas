using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Facebook.Unity
{
    /// <summary>
    /// A result.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error string from the result. If no error occured value is null or empty.</value>
        string Error { get; }
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>A collection of key values pairs that are parsed from the result</value>
        IDictionary<string, object> ResultDictionary { get; }
        /// <summary>
        /// Gets the raw result.
        /// </summary>
        /// <value>The raw result string.</value>
        string RawResult { get; }
        /// <summary>
        /// Gets a value indicating whether this instance cancelled.
        /// </summary>
        /// <value><c>true</c> if this instance cancelled; otherwise, <c>false</c>.</value>
        bool Cancelled { get; }
    }
}
