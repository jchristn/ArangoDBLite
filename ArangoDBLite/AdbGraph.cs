using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ArangoDBLite
{
    /// <summary>
    /// ArangoDB graph.
    /// </summary>
    public class AdbGraph
    {
        #region Public-Members

        /// <summary>
        /// Id.
        /// </summary>
        [JsonProperty("_id")]
        public string Id { get; set; } = null;

        /// <summary>
        /// Key.
        /// </summary>
        [JsonProperty("_key")]
        public string Key { get; set; } = null;

        /// <summary>
        /// Revision.
        /// </summary>
        [JsonProperty("_rev")]
        public string Revision { get; set; } = null;
        
        /// <summary>
        /// Name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = null;

        /// <summary>
        /// Edge definitions.
        /// </summary>
        [JsonProperty("edgeDefinitions")]
        public List<AdbEdgeDefinition> EdgeDefinitions { get; set; } = new List<AdbEdgeDefinition>();

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public AdbGraph()
        {

        }
        
        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="edgeDefinitions">Edge definitions.</param>
        public AdbGraph(string name, List<AdbEdgeDefinition> edgeDefinitions = null)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            Name = name;
            EdgeDefinitions = edgeDefinitions;
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
