using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ArangoDBLite
{
    /// <summary>
    /// ArangoDB edge definition.
    /// </summary>
    public class AdbEdgeDefinition
    {
        #region Public-Members

        /// <summary>
        /// Edge collection name.
        /// </summary>
        [JsonProperty("collection")]
        public string CollectionName { get; set; } = null;

        /// <summary>
        /// Vertex collections from which the edge may initiate.
        /// </summary>
        [JsonProperty("from")]
        public List<string> FromVertexCollections { get; set; } = new List<string>();

        /// <summary>
        /// Vertex collections to which the edge may terminate.
        /// </summary>
        [JsonProperty("to")]
        public List<string> ToVertexCollections { get; set; } = new List<string>();

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public AdbEdgeDefinition()
        {

        }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="collectionName">Edge collection name.</param>
        /// <param name="fromVertexCollections">Vertex collections from which the edge may initiate.</param>
        /// <param name="toVertexCollections">Vertex collections to which the edge may terminate.</param>
        public AdbEdgeDefinition(string collectionName, List<string> fromVertexCollections, List<string> toVertexCollections)
        {
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            if (fromVertexCollections == null) throw new ArgumentNullException(nameof(fromVertexCollections));
            if (toVertexCollections == null) throw new ArgumentNullException(nameof(toVertexCollections));
            if (fromVertexCollections.Count < 1) throw new ArgumentException("At least one vertex collection must be specified for both 'from' and 'to' directions.");
            if (toVertexCollections.Count < 1) throw new ArgumentException("At least one vertex collection must be specified for both 'from' and 'to' directions.");

            CollectionName = collectionName;
            FromVertexCollections = fromVertexCollections;
            ToVertexCollections = toVertexCollections;
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
