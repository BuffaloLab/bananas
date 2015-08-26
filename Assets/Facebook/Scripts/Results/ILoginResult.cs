using System;

namespace Facebook.Unity
{
    /// <summary>
    /// The result of a login request.
    /// </summary>
    public interface ILoginResult : IResult
    {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>The access token returned from login if successful else null.</value>
        AccessToken AccessToken{ get; }
    }
}
