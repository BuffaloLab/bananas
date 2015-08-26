using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Facebook.Unity 
{
    /// <summary>
    /// Contains the access token and related information.
    /// </summary>
    public class AccessToken {

        /// <summary>
        /// Gets or sets the current access token.
        /// </summary>
        /// <value>The current access token.</value>
        public static AccessToken CurrentAccessToken { get; internal set; }

        /// <summary>
        /// Gets or sets the token string.
        /// </summary>
        /// <value>The token string.</value>
        public string TokenString { get; internal set; }
        /// <summary>
        /// Gets or sets the expiration time.
        /// </summary>
        /// <value>The expiration time.</value>
        public DateTime ExpirationTime { get; internal set; }
        /// <summary>
        /// Gets or sets the list of permissions.
        /// </summary>
        /// <value>The permissions.</value>
        public ICollection<string> Permissions { get; internal set; }
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        public string UserId { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Facebook.AccessToken"/> class.
        /// </summary>
        /// <param name="tokenString">Token string.</param>
        /// <param name="userId">User identifier.</param>
        /// <param name="expirationTime">Expiration time.</param>
        /// <param name="permissions">Permissions.</param>
        public AccessToken(
            string tokenString, 
            string userId, 
            DateTime expirationTime,
            ICollection<string> permissions)
        {
            if (String.IsNullOrEmpty(tokenString)) {
                throw new ArgumentNullException("tokenString");
            }

            if (String.IsNullOrEmpty(userId)) {
                throw new ArgumentNullException("userId");
            }

            if (expirationTime == DateTime.MinValue) {
                throw new ArgumentException("Expiration time is unassigned");
            }

            if (permissions == null) {
                throw new ArgumentNullException("permissions");
            }

            this.TokenString = tokenString;
            this.ExpirationTime = expirationTime;
            this.Permissions = permissions;
            this.UserId = userId;
        }
    }
}
