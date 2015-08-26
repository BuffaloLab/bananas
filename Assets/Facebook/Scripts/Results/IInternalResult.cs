using System;

namespace Facebook.Unity
{
    internal interface IInternalResult : IResult
    {
        /// <summary>
        /// Gets the callback identifier.
        /// </summary>
        /// <value>A unique ID for this callback. This value should only be used internally.</value>
        string CallbackId { get; }
    }
}
