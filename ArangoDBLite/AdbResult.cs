using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestWrapper;

namespace ArangoDBLite
{
    /// <summary>
    /// ArangoDBLite API execution result.
    /// </summary>
    public class AdbResult
    {
        #region Public-Members

        /// <summary>
        /// Timestamps associated with the query.
        /// </summary>
        public Timestamps Time { get; set; } = new Timestamps();

        /// <summary>
        /// Flag to indicate if an error was detected.
        /// </summary>
        public bool Error { get; set; } = false;

        /// <summary>
        /// Error message.
        /// </summary>
        public string ErrorMessage { get; set; } = null;

        /// <summary>
        /// Error number.
        /// </summary>
        public int? ErrorNumber { get; set; } = null;

        /// <summary>
        /// HTTP status code.
        /// </summary>
        public int Code { get; set; } = 0;

        /// <summary>
        /// Result.
        /// </summary>
        public object Result { get; set; } = null;

        /// <summary>
        /// List of graphs.
        /// </summary>
        public List<AdbGraph> Graphs { get; set; } = null;

        /// <summary>
        /// Graph.
        /// </summary>
        public AdbGraph Graph { get; set; } = null;

        /// <summary>
        /// Flag that indicates if the search can be continued using the specified cursor ID.
        /// </summary>
        public bool? MoreResults { get; set; } = null;

        /// <summary>
        /// Cursor ID to continue the search.
        /// </summary>
        public string CursorId { get; set; } = null;

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public AdbResult()
        {

        }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="resp">REST response.</param>
        internal AdbResult(RestResponse resp)
        {
            if (resp == null)
            {
                Error = true;
                ErrorMessage = "could not connect";
                ErrorNumber = 0;
            }
            else
            {
                Code = resp.StatusCode;
                if (Code >= 200 && Code <= 299) Error = false;

                if (resp.ContentLength > 0 && !String.IsNullOrEmpty(resp.DataAsString))
                {
                    JObject j = JObject.Parse(resp.DataAsString);
                    if (j.ContainsKey("error") && j["error"] != null) Error = Convert.ToBoolean(j["error"]);
                    if (j.ContainsKey("errorMessage") && j["errorMessage"] != null) ErrorMessage = j["errorMessage"].ToString();
                    if (j.ContainsKey("errorNum") && j["errorNum"] != null) ErrorNumber = Convert.ToInt32(j["errorNum"]);
                    if (j.ContainsKey("hasMore") && j["hasMore"] != null) MoreResults = Convert.ToBoolean(j["hasMore"]);
                    if (j.ContainsKey("id") && j["id"] != null) CursorId = j["id"].ToString();
                } 
            }
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
