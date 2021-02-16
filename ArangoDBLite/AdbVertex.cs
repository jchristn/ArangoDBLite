using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArangoDBLite
{
    /// <summary>
    /// ArangoDB vertex.
    /// </summary>
    public class AdbVertex
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
        /// Old revision.
        /// </summary>
        [JsonProperty("_oldRev")]
        public string OldRevision { get; set; } = null;

        /// <summary>
        /// JObject.
        /// </summary>
        public JObject Data { get; set; } = null;

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public AdbVertex()
        {

        }

        internal AdbVertex(JObject j)
        {
            if (j == null) throw new ArgumentNullException(nameof(j));
            if (!j.ContainsKey("vertex")) throw new ArgumentException("Supplied JSON object does not contain key 'vertex'.");

            if (j.ContainsKey("vertex"))
            { 
                JToken id = j.SelectToken("vertex._id");
                if (id != null) Id = id.ToString();

                JToken key = j.SelectToken("vertex._key");
                if (key != null) Key = key.ToString();

                JToken rev = j.SelectToken("vertex._rev");
                if (rev != null) Revision = rev.ToString();

                JToken oldRev = j.SelectToken("vertex._oldRev");
                if (oldRev != null) OldRevision = oldRev.ToString();
            }

            Data = j["vertex"].ToObject<JObject>();
            if (Data.ContainsKey("_id")) Data.Remove("_id");
            if (Data.ContainsKey("_key")) Data.Remove("_key");
            if (Data.ContainsKey("_rev")) Data.Remove("_rev");
            if (Data.ContainsKey("_oldRev")) Data.Remove("_oldRev");
        }
         
        internal static AdbVertex FromCursorQuery(JObject j)
        {
            AdbVertex ret = new AdbVertex();

            JToken id = j.SelectToken("_id");
            if (id != null) ret.Id = id.ToString();

            JToken key = j.SelectToken("_key");
            if (key != null) ret.Key = key.ToString();

            JToken rev = j.SelectToken("_rev");
            if (rev != null) ret.Revision = rev.ToString();

            JToken oldRev = j.SelectToken("_oldRev");
            if (oldRev != null) ret.OldRevision = oldRev.ToString();
             
            ret.Data = j;

            if (ret.Data.ContainsKey("_id")) ret.Data.Remove("_id");
            if (ret.Data.ContainsKey("_key")) ret.Data.Remove("_key");
            if (ret.Data.ContainsKey("_rev")) ret.Data.Remove("_rev");
            if (ret.Data.ContainsKey("_oldRev")) ret.Data.Remove("_oldRev");

            return ret;
        }

        #endregion

        #region Public-Methods

        /// <summary>
        /// Create an object of the supplied type using the Data property.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <returns>Instance.</returns>
        public T ToObject<T>()
        {
            return Data.ToObject<T>();
        }

        #endregion

        #region Private-Methods

        #endregion 
    }
}
