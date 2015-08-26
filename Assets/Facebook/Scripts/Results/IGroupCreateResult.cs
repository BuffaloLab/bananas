using System;

namespace Facebook.Unity
{
    /// <summary>
    /// The result of a group creation.
    /// </summary>
    public interface IGroupCreateResult : IResult
    {
        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        /// <value>The group id of the group created.</value>
        string GroupId { get; }
    }
}
