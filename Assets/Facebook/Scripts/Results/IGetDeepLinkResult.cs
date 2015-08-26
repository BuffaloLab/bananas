using System;
namespace Facebook.Unity
{
    public interface IGetDeepLinkResult : IResult
    {
        /// <summary>
        /// Gets the deep link.
        /// </summary>
        /// <value>The deep link.</value>
        string DeepLink { get; }
    }
}
