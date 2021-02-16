using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ArangoDBLite
{
    /// <summary>
    /// ArangoDB collection.
    /// Certain parameters are not yet supported, including schema, keyOptions.
    /// </summary>
    public class AdbCollection
    {
        #region Public-Members

        /// <summary>
        /// Name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null;

        /// <summary>
        /// Wait for sync.
        /// </summary>
        [JsonProperty("waitForSync")]
        public bool WaitForSync { get; set; } = true;

        /// <summary>
        /// Do compact.
        /// </summary>
        [JsonProperty("doCompact")]
        public bool DoCompact { get; set; } = true;

        /// <summary>
        /// Journal size.
        /// </summary>
        [JsonProperty("journalSize")]
        public int JournalSize
        {
            get
            {
                return _JournalSize;
            }
            set
            {
                if (value < 1048576) throw new ArgumentException("JournalSize must be at least 1048576.");
                _JournalSize = value;
            }
        }

        /// <summary>
        /// Indicates if the collection is a system collection.
        /// </summary>
        [JsonProperty("isSystem")]
        public bool IsSystem { get; set; } = false; 

        /// <summary>
        /// Indicates if the collection is volatile, i.e. only stored in memory.
        /// </summary>
        [JsonProperty("isVolatile")]
        public bool IsVolatile { get; set; } = false;

        /// <summary>
        /// Collection type.
        /// </summary>
        [JsonProperty("type")]
        public AdbCollectionType CollectionType { get; set; } = AdbCollectionType.DocumentCollection;

        /// <summary>
        /// Globally-unique identifier.
        /// </summary>
        [JsonProperty("globallyUniqueId")]
        public string GloballyUniqueId { get; set; } = null;

        #endregion

        #region Private-Members

        private int _JournalSize = 1048576;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public AdbCollection()
        {

        }
         
        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="collectionType">Collection type.</param>
        /// <param name="waitForSync">Wait for sync.</param>
        /// <param name="doCompact">Do compact.</param>
        /// <param name="journalSize">Journal size.</param>
        /// <param name="isSystem">Is system.</param>
        public AdbCollection(string name, AdbCollectionType collectionType = AdbCollectionType.DocumentCollection, bool waitForSync = true, bool doCompact = true, int journalSize = (1024 * 1024), bool isSystem = false)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            Name = name;
            CollectionType = collectionType;
            WaitForSync = waitForSync;
            DoCompact = doCompact;
            JournalSize = journalSize;
            IsSystem = isSystem;
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
