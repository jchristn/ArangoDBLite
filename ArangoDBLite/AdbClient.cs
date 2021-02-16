using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestWrapper;

namespace ArangoDBLite
{
    /// <summary>
    /// Client for ArangoDB.
    /// </summary>
    public class AdbClient
    {
        #region Public-Members

        /// <summary>
        /// Method to invoke when sending log messages.
        /// </summary>
        public Action<string> Logger = null;

        /// <summary>
        /// When 'Logger' is set, indicate if HTTP requests and responses should also be logged.
        /// </summary>
        public bool LogHttpRequests { get; set; } = false;

        #endregion

        #region Private-Members

        private string _Header = "[ArangoDBLite] ";
        private string _Username = "root";
        private string _Password = null;
        private string _Url = "http://localhost:8529/";

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="baseUrl">Base URL for ArangoDB, i.e. http://localhost:8529/</param>
        public AdbClient(string username = "root", string password = null, string baseUrl = "http://localhost:8529/")
        {
            _Username = username ?? throw new ArgumentNullException(nameof(username));
            _Password = password;
            _Url = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));

            if (!_Url.EndsWith("/")) _Url += "/";
        }

        #endregion

        #region Public-Methods

        #region Database

        /// <summary>
        /// List databases available within the ArangoDB instance.
        /// </summary>
        /// <returns>AdbResult with Result containing list of database names.</returns>
        public async Task<AdbResult> ListDatabases()
        {
            DateTime start = DateTime.Now;
            string url = _Url + "_api/database";
            Logger?.Invoke(_Header + "retrieving database list using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("result") && j["result"] != null)
                {
                    ret.Result = j["result"].ToObject<List<string>>();
                }
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Check for the existence of a database by name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>AdbResult with Result containing a Boolean indicating if the database exists.</returns>
        public async Task<AdbResult> DatabaseExists(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/database";
            Logger?.Invoke(_Header + "checking existence of database list using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            ret.Result = false;

            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("result") && j["result"] != null)
                {
                    List<string> names = j["result"].ToObject<List<string>>();
                    if (names.Contains(name)) ret.Result = true;
                }
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Create a database.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="users">Users to create along with the database.</param>
        /// <param name="options">Database options.</param>
        /// <returns>AdbResult with Result containing Boolean indicating success.</returns>
        public async Task<AdbResult> CreateDatabase(string name, List<AdbUser> users = null, AdbDatabaseOptions options = null)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            AdbDatabase db = new AdbDatabase(name, users, options);
            string url = _Url + "_api/database";
            Logger?.Invoke(_Header + "creating database " + name + " using POST " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.POST, db.ToJson(true));
            AdbResult ret = new AdbResult(resp);
            ret.Result = !ret.Error;

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Delete a database.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>AdbResult with Result containing Boolean indicating success.</returns>
        public async Task<AdbResult> DeleteDatabase(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/database/" + name;
            Logger?.Invoke(_Header + "deleting database " + name + " using DELETE " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.DELETE);
            AdbResult ret = new AdbResult(resp);
            ret.Result = !ret.Error;

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        #endregion

        #region Users

        /// <summary>
        /// List users available within the ArangoDB instance.
        /// </summary>
        /// <returns>AdbResult with Result containing list of AdbUser objects.</returns>
        public async Task<AdbResult> ListUsers()
        {
            DateTime start = DateTime.Now;
            string url = _Url + "_api/user";
            Logger?.Invoke(_Header + "retrieving user list using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("result") && j["result"] != null) ret.Result = j["result"].ToObject<List<AdbUser>>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Retrieve a user.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>AdbResult with Result containing an AdbUser object.</returns>
        public async Task<AdbResult> GetUser(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/user/" + name;
            Logger?.Invoke(_Header + "retrieving user " + name + " using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                ret.Result = JObject.Parse(resp.DataAsString).ToObject<AdbUser>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Check for the existence of a user by name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>AdbResult with Result containing a Boolean indicating if the user exists.</returns>
        public async Task<AdbResult> UserExists(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/user/" + name;
            Logger?.Invoke(_Header + "checking existence of user " + name + " using GET " + url);
            RestResponse resp = await GetRestResponse(url);
            
            AdbResult ret = new AdbResult(resp);
            ret.Result = !ret.Error;

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Create a user.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="password">Password.</param>
        /// <param name="active">Flag to indicate if the account is active.</param>
        /// <param name="metadata">Additional metadata parameters.</param>
        /// <returns>AdbResult with Result containing an AdbUser object.</returns>
        public async Task<AdbResult> CreateUser(string username, string password, bool active = true, Dictionary<string, object> metadata = null)
        {
            return await CreateUser(new AdbUser(username, password, active, metadata));
        }

        /// <summary>
        /// Create a user.
        /// </summary>
        /// <param name="user">User.</param>
        /// <returns>AdbResult with Result containing an AdbUser object.</returns>
        public async Task<AdbResult> CreateUser(AdbUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/user";
            Logger?.Invoke(_Header + "creating user " + user.Username + " using POST " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.POST, user.ToJson(true));

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                ret.Result = JObject.Parse(resp.DataAsString).ToObject<AdbUser>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Delete a user.
        /// </summary>
        /// <param name="name">Username.</param>
        /// <returns>AdbResult with Result indicating success.</returns>
        public async Task<AdbResult> DeleteUser(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/user/" + name;
            Logger?.Invoke(_Header + "deleting user " + name + " using DELETE " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.DELETE);
            AdbResult ret = new AdbResult(resp);
            ret.Result = !ret.Error;

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        #endregion

        #region Collections

        /// <summary>
        /// List collections.
        /// </summary>
        /// <returns>AdbResult with Result containing list of AdbCollection objects.</returns>
        public async Task<AdbResult> ListCollections()
        {
            DateTime start = DateTime.Now;
            string url = _Url + "_api/collection";
            Logger?.Invoke(_Header + "retrieving collection list using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("result") && j["result"] != null) ret.Result = j["result"].ToObject<List<AdbCollection>>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// List collections by collection type.
        /// </summary>
        /// <param name="collectionType">Collection type.</param>
        /// <returns>AdbResult with Result containing list of AdbCollection objects.</returns>
        public async Task<AdbResult> ListCollections(AdbCollectionType collectionType = AdbCollectionType.DocumentCollection)
        {
            DateTime start = DateTime.Now;
            string url = _Url + "_api/collection";
            Logger?.Invoke(_Header + "retrieving collection list using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("result") && j["result"] != null)
                {
                    List<AdbCollection> collections = j["result"].ToObject<List<AdbCollection>>();
                    if (collections != null && collections.Count > 0)
                    {
                        ret.Result = collections.Where(c => c.CollectionType.Equals(collectionType)).ToList();
                    }
                }
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Retrieve a collection.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>AdbResult with Result containing an AdbCollection object.</returns>
        public async Task<AdbResult> GetCollection(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/collection/" + name;
            Logger?.Invoke(_Header + "retrieving collection " + name + " using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                ret.Result = JObject.Parse(resp.DataAsString).ToObject<AdbCollection>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Check for the existence of a collection by name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>AdbResult with Result containing a Boolean indicating if the collection exists.</returns>
        public async Task<AdbResult> CollectionExists(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/collection/" + name;
            Logger?.Invoke(_Header + "checking existence of collection " + name + " using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            ret.Result = !ret.Error;

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Create a collection.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="collectionType">Collection type.</param>
        /// <param name="waitForSync">Wait for sync.</param>
        /// <param name="doCompact">Do compact.</param>
        /// <param name="journalSize">Journal size.</param>
        /// <param name="isSystem">Is system.</param>
        /// <returns>AdbResult with Result containing an AdbCollection object.</returns>
        public async Task<AdbResult> CreateCollection(string name, AdbCollectionType collectionType = AdbCollectionType.DocumentCollection, bool waitForSync = true, bool doCompact = true, int journalSize = (1024 * 1024), bool isSystem = false)
        {
            return await CreateCollection(new AdbCollection(name, collectionType, waitForSync, doCompact, journalSize, isSystem));
        }

        /// <summary>
        /// Create a collection.
        /// </summary>
        /// <param name="collection">Collection.</param>
        /// <returns>AdbResult with Result containing an AdbCollection object.</returns>
        public async Task<AdbResult> CreateCollection(AdbCollection collection)
        {
            if (collection == null) throw new ArgumentNullException(nameof(collection));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/collection";
            Logger?.Invoke(_Header + "creating collection " + collection.Name + " using POST " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.POST, collection.ToJson(true));

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                ret.Result = JObject.Parse(resp.DataAsString).ToObject<AdbCollection>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Delete a collection.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>AdbResult with Result indicating success.</returns>
        public async Task<AdbResult> DeleteCollection(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/collection/" + name;
            Logger?.Invoke(_Header + "deleting collection " + name + " using DELETE " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.DELETE);
            AdbResult ret = new AdbResult(resp);
            ret.Result = !ret.Error;

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        #endregion

        #region Graphs

        /// <summary>
        /// List graphs.
        /// </summary>
        /// <returns>AdbResult with Graphs containing list of AdbGraph objects.</returns>
        public async Task<AdbResult> ListGraphs()
        {
            DateTime start = DateTime.Now;
            string url = _Url + "_api/gharial";
            Logger?.Invoke(_Header + "retrieving graph list using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("graphs") && j["graphs"] != null) ret.Graphs = j["graphs"].ToObject<List<AdbGraph>>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Retrieve a graph.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>AdbResult with Graph containing an AdbGraph object.</returns>
        public async Task<AdbResult> GetGraph(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/gharial/" + name;
            Logger?.Invoke(_Header + "retrieving graph " + name + " using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("graph") && j["graph"] != null) ret.Graph = j["graph"].ToObject<AdbGraph>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Check for the existence of a graph by name.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>AdbResult with Result containing a Boolean indicating if the graph exists.</returns>
        public async Task<AdbResult> GraphExists(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/gharial/" + name;
            Logger?.Invoke(_Header + "checking existence of graph " + name + " using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            ret.Result = !ret.Error;

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Create a graph.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="edgeDefinitions">Edge definitions.</param>
        /// <returns>AdbResult with Graph containing an AdbGraph object.</returns>
        public async Task<AdbResult> CreateGraph(string name, List<AdbEdgeDefinition> edgeDefinitions = null)
        {
            return await CreateGraph(new AdbGraph(name, edgeDefinitions));
        }

        /// <summary>
        /// Create a graph.
        /// </summary>
        /// <param name="graph">Graph.</param>
        /// <returns>AdbResult with Graph containing an AdbGraph object.</returns>
        public async Task<AdbResult> CreateGraph(AdbGraph graph)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/gharial";
            Logger?.Invoke(_Header + "creating graph " + graph.Name + " using POST " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.POST, graph.ToJson(true));

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("graph") && j["graph"] != null) ret.Graph = j["graph"].ToObject<AdbGraph>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Add edge collection to a graph.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="edgeDefinition">Edge definition.</param>
        /// <returns>AdbResult with Graph containing an AdbGraph object.</returns>
        public async Task<AdbResult> AddGraphEdgeCollection(string name, AdbEdgeDefinition edgeDefinition)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (edgeDefinition == null) throw new ArgumentNullException(nameof(edgeDefinition));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/gharial/" + name + "/edge";
            Logger?.Invoke(_Header + "creating edge definition on graph " + name + " using POST " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.POST, edgeDefinition.ToJson(true));

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("graph") && j["graph"] != null) ret.Graph = j["graph"].ToObject<AdbGraph>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Delete a graph.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <returns>AdbResult with Result indicating success.</returns>
        public async Task<AdbResult> DeleteGraph(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/gharial/" + name;
            Logger?.Invoke(_Header + "deleting graph " + name + " using DELETE " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.DELETE);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("removed") && j["removed"] != null) ret.Result = Convert.ToBoolean(j["removed"]);
                else ret.Result = false;
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        #endregion

        #region Vertices

        /// <summary>
        /// List vertices.
        /// Note: this API uses cursor queries iteratively to retrieve all results.  
        /// As such, it may take a long time to complete, and has the potential to consume a lot of memory.  
        /// It is recommended that you use filters to reduce the number of results returned.
        /// Filters are ANDed within the query.
        /// </summary>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="filters">Filters to include in the cursor query.</param>
        /// <returns>AdbResult where Result contains a list of AdbVertex objects.</returns>
        public async Task<AdbResult> ListVertices(string collectionName, Dictionary<string, string> filters = null)
        {
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));

            DateTime start = DateTime.Now;
            AdbResult ret = new AdbResult();
            string cursorId = null;

            string query = "FOR d IN " + collectionName + " ";

            int filtersAdded = 0;
            if (filters != null && filters.Count > 0)
            {
                foreach (KeyValuePair<string, string> filter in filters)
                {
                    if (String.IsNullOrEmpty(filter.Key)) continue;
                    if (String.IsNullOrEmpty(filter.Value)) continue;
                    if (filtersAdded == 0) query += "FILTER ";
                    if (filtersAdded > 0) query += "AND ";
                    query += "d." + filter.Key + " == '" + filter.Value + "' ";
                    filtersAdded++;
                }
            }

            query += "RETURN d";

            List<AdbVertex> vertices = new List<AdbVertex>();

            while (true)
            {
                AdbResult r = await ExecuteCursorQuery<AdbVertex>(query, cursorId);
                ret.Code = r.Code;
                ret.Error = r.Error;
                ret.ErrorMessage = r.ErrorMessage;

                if (r.Error) break;
                if (r.Result != null)
                {
                    foreach (AdbVertex e in (List<AdbVertex>)r.Result)
                    {
                        vertices.Add(e);
                    }
                }

                cursorId = r.CursorId;
                if (r.MoreResults == null || !r.MoreResults.Value) break;
            }

            ret.Result = vertices;
            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Create a vertex.
        /// </summary>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="json">JSON.</param>
        /// <returns>AdbResult with Result containing an AdbVertex object.</returns>
        public async Task<AdbResult> CreateVertex(string graphName, string collectionName, string json)
        {
            if (String.IsNullOrEmpty(graphName)) throw new ArgumentNullException(nameof(graphName));
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            if (String.IsNullOrEmpty(json)) throw new ArgumentNullException(nameof(json));

            DateTime start = DateTime.Now;
            JObject o = JObject.Parse(json);
            string url = _Url + "_api/gharial/" + graphName + "/vertex/" + collectionName;
            Logger?.Invoke(_Header + "creating vertex in " + graphName + "/" + collectionName + " using POST " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.POST, o.ToJson(true));

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("vertex") && j["vertex"] != null) ret.Result = j["vertex"].ToObject<AdbVertex>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Create a vertex.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="obj">Object.  Must be serializable.</param>
        /// <returns>AdbResult with Result containing an AdbVertex object.</returns>
        public async Task<AdbResult> CreateVertex<T>(string graphName, string collectionName, T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return await CreateVertex(graphName, collectionName, obj.ToJson(false));
        }

        /// <summary>
        /// Update an existing vertex.
        /// </summary>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="key">Key.</param>
        /// <param name="json">JSON.</param>
        /// <returns>AdbResult with Result containing an AdbVertex object.</returns>
        public async Task<AdbResult> UpdateVertex(string graphName, string collectionName, string key, string json)
        {
            if (String.IsNullOrEmpty(graphName)) throw new ArgumentNullException(nameof(graphName));
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (String.IsNullOrEmpty(json)) throw new ArgumentNullException(nameof(json));

            DateTime start = DateTime.Now;
            JObject o = JObject.Parse(json);
            string url = _Url + "_api/gharial/" + graphName + "/vertex/" + collectionName + "/" + key;
            Logger?.Invoke(_Header + "updating vertex in " + graphName + "/" + collectionName + "/" + key + " using PATCH " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.PATCH, o.ToJson(true));

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("vertex") && j["vertex"] != null) ret.Result = j["vertex"].ToObject<AdbVertex>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Overwrite an existing vertex.
        /// </summary>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="key">Key.</param>
        /// <param name="json">JSON.</param>
        /// <returns>AdbResult with Result containing an AdbVertex object.</returns>
        public async Task<AdbResult> ReplaceVertex(string graphName, string collectionName, string key, string json)
        {
            if (String.IsNullOrEmpty(graphName)) throw new ArgumentNullException(nameof(graphName));
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (String.IsNullOrEmpty(json)) throw new ArgumentNullException(nameof(json));

            DateTime start = DateTime.Now;
            JObject o = JObject.Parse(json);
            string url = _Url + "_api/gharial/" + graphName + "/vertex/" + collectionName + "/" + key;
            Logger?.Invoke(_Header + "replacing vertex in " + graphName + "/" + collectionName + "/" + key + " using PUT " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.PUT, o.ToJson(true));

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("vertex") && j["vertex"] != null) ret.Result = j["vertex"].ToObject<AdbVertex>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Retrieve a vertex.
        /// </summary>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="key">Key.</param>
        /// <returns>AdbResult with Result containing an AdbVertex object.</returns>
        public async Task<AdbResult> GetVertex(string graphName, string collectionName, string key)
        {
            if (String.IsNullOrEmpty(graphName)) throw new ArgumentNullException(nameof(graphName));
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/gharial/" + graphName + "/vertex/" + collectionName + "/" + key;
            Logger?.Invoke(_Header + "retrieving vertex " + graphName + "/" + collectionName + "/" + key + " using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("vertex") && j["vertex"] != null) ret.Result = j["vertex"].ToObject<AdbVertex>();
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Delete a vertex.
        /// </summary>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="key">Key.</param>
        /// <returns>AdbResult with Result indicating success.</returns>
        public async Task<AdbResult> DeleteVertex(string graphName, string collectionName, string key)
        {
            if (String.IsNullOrEmpty(graphName)) throw new ArgumentNullException(nameof(graphName));
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/gharial/" + graphName + "/vertex/" + collectionName + "/" + key;
            Logger?.Invoke(_Header + "deleting vertex " + graphName + "/" + collectionName + "/" + key + " using DELETE " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.DELETE);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("removed") && j["removed"] != null) ret.Result = Convert.ToBoolean(j["removed"]);
                else ret.Result = false;
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Retrieve the first vertex matching the supplied filters.
        /// Note: this API uses cursor queries internally.
        /// Filters are ANDed within the query.
        /// </summary>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="filters">Filters to include in the cursor query.</param>
        /// <returns>AdbResult where Result contains the first matching AdbVertex object.</returns>
        public async Task<AdbResult> FirstVertex(string collectionName, Dictionary<string, string> filters = null)
        {
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));

            DateTime start = DateTime.Now;
            AdbResult ret = new AdbResult(); 

            string query = "FOR d IN " + collectionName + " ";

            int filtersAdded = 0;
            if (filters != null && filters.Count > 0)
            {
                foreach (KeyValuePair<string, string> filter in filters)
                {
                    if (String.IsNullOrEmpty(filter.Key)) continue;
                    if (String.IsNullOrEmpty(filter.Value)) continue;
                    if (filtersAdded == 0) query += "FILTER ";
                    if (filtersAdded > 0) query += "AND ";
                    query += "d." + filter.Key + " == '" + filter.Value + "' ";
                    filtersAdded++;
                }
            }

            query += "RETURN d";

            AdbResult r = await ExecuteCursorQuery<JArray>(query);
            ret.Code = r.Code;
            ret.Error = r.Error;
            ret.ErrorMessage = r.ErrorMessage;

            if (!ret.Error && r.Result != null)
            {
                JArray array = r.Result as JArray;
                foreach (JObject j in array)
                {
                    ret.Result = AdbVertex.FromCursorQuery(j);
                    break;
                }
            } 

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        #endregion

        #region Edges

        /// <summary>
        /// List edges.
        /// Note: this API uses cursor queries iteratively to retrieve all results.  
        /// As such, it may take a long time to complete, and has the potential to consume a lot of memory.  
        /// It is recommended that you use filters to reduce the number of results returned.
        /// Filters are ANDed within the query.
        /// </summary>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="filters">Filters to include in the cursor query.</param>
        /// <returns>AdbResult where Result contains a list of AdbEdge objects.</returns>
        public async Task<AdbResult> ListEdges(string collectionName, Dictionary<string, string> filters = null)
        {
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));

            DateTime start = DateTime.Now;
            AdbResult ret = new AdbResult();
            string cursorId = null;

            string query = "FOR d IN " + collectionName + " ";

            int filtersAdded = 0;
            if (filters != null && filters.Count > 0)
            {
                foreach (KeyValuePair<string, string> filter in filters)
                {
                    if (String.IsNullOrEmpty(filter.Key)) continue;
                    if (String.IsNullOrEmpty(filter.Value)) continue;
                    if (filtersAdded == 0) query += "FILTER ";
                    if (filtersAdded > 0) query += "AND ";
                    query += "d." + filter.Key + " == '" + filter.Value + "' ";
                    filtersAdded++;
                }
            }

            query += "RETURN d";

            List<AdbEdge> edges = new List<AdbEdge>();

            while (true)
            {
                AdbResult r = await ExecuteCursorQuery<AdbEdge>(query, cursorId);
                ret.Code = r.Code;
                ret.Error = r.Error;
                ret.ErrorMessage = r.ErrorMessage;

                if (r.Error) break;
                if (r.Result != null)
                { 
                    foreach (AdbEdge e in (List<AdbEdge>)r.Result)
                    {
                        edges.Add(e);
                    }
                }

                cursorId = r.CursorId;
                if (r.MoreResults == null || !r.MoreResults.Value) break;
            }

            ret.Result = edges;
            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Create an edge.
        /// </summary>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="fromId">From ID.</param>
        /// <param name="toId">To ID.</param>
        /// <param name="json">JSON.</param>
        /// <returns>AdbResult with Result containing an AdbEdge object.</returns>
        public async Task<AdbResult> CreateEdge(string graphName, string collectionName, string fromId, string toId, string json)
        {
            if (String.IsNullOrEmpty(graphName)) throw new ArgumentNullException(nameof(graphName));
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            if (String.IsNullOrEmpty(fromId)) throw new ArgumentNullException(nameof(fromId));
            if (String.IsNullOrEmpty(toId)) throw new ArgumentNullException(nameof(toId));
            if (String.IsNullOrEmpty(json)) throw new ArgumentNullException(nameof(json));

            DateTime start = DateTime.Now;
            JObject o = JObject.Parse(json);
            o["_from"] = fromId;
            o["_to"] = toId;

            string url = _Url + "_api/gharial/" + graphName + "/edge/" + collectionName;
            Logger?.Invoke(_Header + "creating edge in " + graphName + "/" + collectionName + " using POST " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.POST, o.ToJson(true));

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                ret.Result = new AdbEdge(j);
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Create an edge.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="fromId">From ID.</param>
        /// <param name="toId">To ID.</param>
        /// <param name="obj">Object.  Must be serializable.</param>
        /// <returns>AdbResult with Result containing an AdbEdge object.</returns>
        public async Task<AdbResult> CreateEdge<T>(string graphName, string collectionName, string fromId, string toId, T obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            return await CreateEdge(graphName, collectionName, fromId, toId, obj.ToJson(false));
        }

        /// <summary>
        /// Update an edge.
        /// </summary>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="key">Key.</param>
        /// <param name="json">JSON.</param>
        /// <returns>AdbResult with Result containing an AdbEdge object.</returns>
        public async Task<AdbResult> UpdateEdge(string graphName, string collectionName, string key, string json)
        {
            if (String.IsNullOrEmpty(graphName)) throw new ArgumentNullException(nameof(graphName));
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (String.IsNullOrEmpty(json)) throw new ArgumentNullException(nameof(json));

            DateTime start = DateTime.Now;
            JObject o = JObject.Parse(json);
            string url = _Url + "_api/gharial/" + graphName + "/edge/" + collectionName + "/" + key;
            Logger?.Invoke(_Header + "updating edge in " + graphName + "/" + collectionName + "/" + key + " using PATCH " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.PATCH, o.ToJson(true));

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                ret.Result = new AdbEdge(j);
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Replace an edge.
        /// </summary>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="key">Key.</param>
        /// <param name="json">JSON.</param>
        /// <returns>AdbResult with Result containing an AdbEdge object.</returns>
        public async Task<AdbResult> ReplaceEdge(string graphName, string collectionName, string key, string json)
        {
            if (String.IsNullOrEmpty(graphName)) throw new ArgumentNullException(nameof(graphName));
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (String.IsNullOrEmpty(json)) throw new ArgumentNullException(nameof(json));

            DateTime start = DateTime.Now;
            JObject o = JObject.Parse(json);
            string url = _Url + "_api/gharial/" + graphName + "/edge/" + collectionName + "/" + key;
            Logger?.Invoke(_Header + "replacing edge in " + graphName + "/" + collectionName + "/" + key + " using PUT " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.PUT, o.ToJson(true));

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                ret.Result = new AdbEdge(j);
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Retrieve an edge.
        /// </summary>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="key">Key.</param>
        /// <returns>AdbResult with Result containing an AdbEdge object.</returns>
        public async Task<AdbResult> GetEdge(string graphName, string collectionName, string key)
        {
            if (String.IsNullOrEmpty(graphName)) throw new ArgumentNullException(nameof(graphName));
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/gharial/" + graphName + "/edge/" + collectionName + "/" + key;
            Logger?.Invoke(_Header + "retrieving edge " + graphName + "/" + collectionName + "/" + key + " using GET " + url);
            RestResponse resp = await GetRestResponse(url);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                ret.Result = new AdbEdge(j);
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Delete an edge.
        /// </summary>
        /// <param name="graphName">Graph name.</param>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="key">Key.</param>
        /// <returns>AdbResult with Result indicating success.</returns>
        public async Task<AdbResult> DeleteEdge(string graphName, string collectionName, string key)
        {
            if (String.IsNullOrEmpty(graphName)) throw new ArgumentNullException(nameof(graphName));
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));
            if (String.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            DateTime start = DateTime.Now;
            string url = _Url + "_api/gharial/" + graphName + "/edge/" + collectionName + "/" + key;
            Logger?.Invoke(_Header + "deleting edge " + graphName + "/" + collectionName + "/" + key + " using DELETE " + url);
            RestResponse resp = await GetRestResponse(url, HttpMethod.DELETE);

            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                JObject j = JObject.Parse(resp.DataAsString);
                if (j.ContainsKey("removed") && j["removed"] != null) ret.Result = Convert.ToBoolean(j["removed"]);
                else ret.Result = false;
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        /// <summary>
        /// Retrieve the first edge matching the supplied filters.
        /// Note: this API uses cursor queries internally.
        /// Filters are ANDed within the query.
        /// </summary>
        /// <param name="collectionName">Collection name.</param>
        /// <param name="filters">Filters to include in the cursor query.</param>
        /// <returns>AdbResult where Result contains the first matching AdbEdge object.</returns>
        public async Task<AdbResult> FirstEdge(string collectionName, Dictionary<string, string> filters = null)
        {
            if (String.IsNullOrEmpty(collectionName)) throw new ArgumentNullException(nameof(collectionName));

            DateTime start = DateTime.Now;
            AdbResult ret = new AdbResult();

            string query = "FOR d IN " + collectionName + " ";

            int filtersAdded = 0;
            if (filters != null && filters.Count > 0)
            {
                foreach (KeyValuePair<string, string> filter in filters)
                {
                    if (String.IsNullOrEmpty(filter.Key)) continue;
                    if (String.IsNullOrEmpty(filter.Value)) continue;
                    if (filtersAdded == 0) query += "FILTER ";
                    if (filtersAdded > 0) query += "AND ";
                    query += "d." + filter.Key + " == '" + filter.Value + "' ";
                    filtersAdded++;
                }
            }

            query += "RETURN d";

            AdbResult r = await ExecuteCursorQuery<JArray>(query);
            ret.Code = r.Code;
            ret.Error = r.Error;
            ret.ErrorMessage = r.ErrorMessage;

            if (!ret.Error && r.Result != null)
            {
                JArray array = r.Result as JArray;
                foreach (JObject j in array)
                {
                    ret.Result = AdbEdge.FromCursorQuery(j);
                    break;
                }
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        #endregion

        #region Cursor-Queries

        /// <summary>
        /// Execute a cursor query.
        /// </summary>
        /// <typeparam name="T">Return value type.</typeparam>
        /// <param name="query">Query.</param>
        /// <param name="cursorId">Cursor ID.</param>
        /// <returns>AdbResult with Result containing an object of the supplied type.</returns>
        public async Task<AdbResult> ExecuteCursorQuery<T>(string query, string cursorId = null) where T : class
        {
            // FOR v IN vertices FILTER v._key == '46986' AND v.foo == 'bar' RETURN v
            if (String.IsNullOrEmpty(query)) throw new ArgumentNullException(nameof(query));

            DateTime start = DateTime.Now;
            Dictionary<string, string> o = new Dictionary<string, string>();
            o.Add("query", query);

            string url = _Url + "_api/cursor";
            HttpMethod method = HttpMethod.POST;
            string data = o.ToJson(true);

            if (!String.IsNullOrEmpty(cursorId))
            {
                url += "/" + cursorId;
                method = HttpMethod.PUT;
                data = null;
            }

            List<AdbEdge> edges = new List<AdbEdge>();
            List<AdbVertex> vertices = new List<AdbVertex>();
            List<T> custom = new List<T>();

            JObject jObj = null;
            JArray jArray = null;

            Logger?.Invoke(_Header + "executing cursor query using " + method.ToString() + " " + url + ": " + Environment.NewLine + query);
            RestResponse resp = await GetRestResponse(url, method, data);
             
            AdbResult ret = new AdbResult(resp);
            if (!ret.Error && !String.IsNullOrEmpty(resp.DataAsString))
            {
                jObj = JObject.Parse(resp.DataAsString); 
                jArray = jObj["result"] as JArray;

                foreach (JObject j in jArray)
                {
                    if (typeof(T) == typeof(AdbEdge))
                    {
                        edges.Add(AdbEdge.FromCursorQuery(j));
                    }
                    else if (typeof(T) == typeof(AdbVertex))
                    {
                        vertices.Add(AdbVertex.FromCursorQuery(j));
                    } 
                    else if (typeof(T) == typeof(JArray))
                    {
                        // do nothing
                    }
                    else if (typeof(T) == typeof(JObject))
                    {
                        // do nothing
                    }
                    else
                    {
                        custom.Add(j.ToObject<T>());
                    }
                }
            }

            if (typeof(T) == typeof(AdbEdge))
            {
                ret.Result = edges;
            }
            else if (typeof(T) == typeof(AdbVertex))
            {
                ret.Result = vertices;
            }
            else if (typeof(T) == typeof(JArray))
            {
                ret.Result = jArray;
            }
            else if (typeof(T) == typeof(JObject))
            {
                ret.Result = jObj;
            }
            else
            {
                ret.Result = custom;
            }

            ret.Time.Start = start;
            ret.Time.End = DateTime.Now;
            return ret;
        }

        #endregion

        #endregion

        #region Private-Methods

        private async Task<RestResponse> GetRestResponse(string url, HttpMethod method = HttpMethod.GET, string body = null)
        {
            Timestamps ts = new Timestamps();
            
            RestRequest req = new RestRequest(url, method);
            req.Authorization.User = _Username;
            req.Authorization.Password = _Password;
            req.Authorization.EncodeCredentials = true;
            if (!String.IsNullOrEmpty(body)) req.ContentType = "application/json";
            if (LogHttpRequests) req.Logger = Logger;

            RestResponse resp = null;

            string baseMsg = null;
            if (!String.IsNullOrEmpty(body)) baseMsg = _Header + body.Length + " bytes to " + method.ToString() + " " + url;
            else baseMsg = _Header + method.ToString() + " " + url;
            
            try
            {
                Logger?.Invoke(baseMsg); 

                if (!String.IsNullOrEmpty(body))
                {
                    resp = await req.SendAsync(body).ConfigureAwait(false);
                }
                else
                {
                    resp = await req.SendAsync().ConfigureAwait(false);
                }

                return resp; 
            }
            catch (Exception e)
            {
                Logger?.Invoke(baseMsg + " exception:" + Environment.NewLine + e.ToJson(true));
                return null;
            }
            finally
            {
                ts.End = DateTime.Now.ToUniversalTime();

                if (resp != null) baseMsg += " " + resp.StatusCode;
                else baseMsg += " (null)";

                if (resp != null && resp.ContentLength > 0) baseMsg += " " + resp.ContentLength + " bytes";

                baseMsg += " (" + ts.TotalMs + "ms)";
                Logger?.Invoke(baseMsg); 
            }
        }

        #endregion
    }
}
