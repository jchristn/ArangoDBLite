using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArangoDBLite
{
    /// <summary>
    /// ArangoDB edge.
    /// </summary>
    public class AdbEdge
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
        /// From.
        /// </summary>
        [JsonProperty("_from")]
        public string From { get; set; } = null;

        /// <summary>
        /// To.
        /// </summary>
        [JsonProperty("_to")]
        public string To { get; set; } = null;

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
        public AdbEdge()
        {

        } 
         
        internal AdbEdge(JObject j)
        {
            if (j == null) throw new ArgumentNullException(nameof(j));
            if (!j.ContainsKey("edge")) throw new ArgumentException("Supplied JSON object does not contain key 'edge'.");

            if (j.ContainsKey("edge"))
            {
                JToken id = j.SelectToken("edge._id");
                if (id != null) Id = id.ToString();

                JToken key = j.SelectToken("edge._key");
                if (key != null) Key = key.ToString();

                JToken rev = j.SelectToken("edge._rev");
                if (rev != null) Revision = rev.ToString();

                JToken oldRev = j.SelectToken("edge._oldRev");
                if (oldRev != null) OldRevision = oldRev.ToString();

                JToken from = j.SelectToken("edge._from");
                if (from != null) From = from.ToString();

                JToken to = j.SelectToken("edge._to");
                if (to != null) To = to.ToString();
            }

            Data = j["edge"].ToObject<JObject>();
            if (Data.ContainsKey("_id")) Data.Remove("_id");
            if (Data.ContainsKey("_key")) Data.Remove("_key");
            if (Data.ContainsKey("_rev")) Data.Remove("_rev");
            if (Data.ContainsKey("_oldRev")) Data.Remove("_oldRev");
            if (Data.ContainsKey("_from")) Data.Remove("_from");
            if (Data.ContainsKey("_to")) Data.Remove("_to");
        }

        internal static AdbEdge FromCursorQuery(JObject j)
        {
            AdbEdge ret = new AdbEdge();

            JToken id = j.SelectToken("_id");
            if (id != null) ret.Id = id.ToString();

            JToken key = j.SelectToken("_key");
            if (key != null) ret.Key = key.ToString();

            JToken rev = j.SelectToken("_rev");
            if (rev != null) ret.Revision = rev.ToString();

            JToken oldRev = j.SelectToken("_oldRev");
            if (oldRev != null) ret.OldRevision = oldRev.ToString();

            JToken from = j.SelectToken("_from");
            if (from != null) ret.From = from.ToString();

            JToken to = j.SelectToken("_to");
            if (to != null) ret.To = to.ToString();

            ret.Data = j;

            if (ret.Data.ContainsKey("_id")) ret.Data.Remove("_id");
            if (ret.Data.ContainsKey("_key")) ret.Data.Remove("_key");
            if (ret.Data.ContainsKey("_rev")) ret.Data.Remove("_rev");
            if (ret.Data.ContainsKey("_oldRev")) ret.Data.Remove("_oldRev");
            if (ret.Data.ContainsKey("_from")) ret.Data.Remove("_from");
            if (ret.Data.ContainsKey("_to")) ret.Data.Remove("_to");

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
