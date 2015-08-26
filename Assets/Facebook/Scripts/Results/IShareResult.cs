using System;

namespace Facebook.Unity
{
    /// <summary>
    /// The result of a share.
    /// </summary>
    public interface IShareResult : IResult
    {
        /// <summary>
        /// Gets the post identifier.
        /// </summary>
        /// <value>The post identifier if the post was successful and the user is tossed.</value>
        string PostId { get; }
    }
}
