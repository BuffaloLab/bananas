using System;
using System.Collections.Generic;

namespace Facebook.Unity
{
    /// <summary>
    /// The result of a graph api call.
    /// </summary>
    public interface IGraphResult : IResult
    {
        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>A list parsed from the result</value>
        IList<object> ResultList { get; }
    }
}
