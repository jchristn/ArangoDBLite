using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ArangoDBLite
{
    /// <summary>
    /// ArangoDB collection type.
    /// </summary>
    public enum AdbCollectionType
    {
        /// <summary>
        /// Document collection.
        /// </summary>
        DocumentCollection = 2,
        /// <summary>
        /// Edge collection.
        /// </summary>
        EdgeCollection = 3
    }
}
