using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ArangoDBLite
{
    /// <summary>
    /// ArangoDB database.
    /// </summary>
    public class AdbDatabase
    {
        #region Public-Members

        /// <summary>
        /// Name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null;

        /// <summary>
        /// Users.
        /// </summary>
        [JsonProperty("users")]
        public List<AdbUser> Users { get; set; } = null;

        /// <summary>
        /// Database options.
        /// </summary>
        [JsonProperty("options")]
        public AdbDatabaseOptions Options { get; set; } = new AdbDatabaseOptions();

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public AdbDatabase()
        {

        }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="users">Users.</param>
        /// <param name="options">Database options.</param>
        public AdbDatabase(string name, List<AdbUser> users = null, AdbDatabaseOptions options = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Users = users;
            Options = options;
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion 
    }
}
