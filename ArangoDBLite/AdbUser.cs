using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ArangoDBLite
{
    /// <summary>
    /// ArangoDB user.
    /// </summary>
    public class AdbUser
    {
        #region Public-Members

        /// <summary>
        /// Username.
        /// </summary>
        [JsonProperty("user")]
        public string Username { get; set; } = null;

        /// <summary>
        /// Password.
        /// </summary>
        [JsonProperty("passwd")]
        public string Password { get; set; } = null;

        /// <summary>
        /// Flag to indicate if the account is active.
        /// </summary>
        [JsonProperty("active")]
        public bool Active { get; set; } = true;

        /// <summary>
        /// Additional metadata parameters.
        /// </summary>
        [JsonProperty("extra")]
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public AdbUser()
        {

        }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="active">Flag to indicate if the account is active.</param>
        /// <param name="metadata">Additional metadata parameters.</param>
        public AdbUser(string username, string password, bool active = true, Dictionary<string, object> metadata = null)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Active = active;
            Metadata = metadata;
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
