using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
    /// <summary>
    /// App request result.
    /// </summary>
    public interface IAppRequestResult : IResult
    {
        /// <summary>
        /// Gets RequestID
        /// </summary>
        /// <value>A request ID assigned by Facebook</value>
        string RequestID { get; }

        /// <summary>
        /// Gets to.
        /// </summary>
        /// <value>An array of string, each element being the Facebook ID of one of the selected recipients.</value>
        IEnumerable<string> To { get; }
    }
}
