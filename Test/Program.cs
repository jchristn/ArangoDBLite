using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using ArangoDBLite;
using Newtonsoft.Json;

namespace Test
{
    class Program
    {
        static bool _RunForever = true;
        static string _Url = null;
        static string _Username = null;
        static string _Password = null;
        static AdbClient _Client = null;

        static void Main(string[] args)
        {
            _Url = InputString("URL:", "http://127.0.0.1:8529/", false);
            _Username = InputString("User:", "root", false);
            _Password = InputString("Password:", null, true);
            _Client = new AdbClient(_Username, _Password, _Url);
            _Client.Logger = Console.WriteLine;
            _Client.LogHttpRequests = true;
             
            while (_RunForever)
            {
                string cmd = InputString("Command [? for help]:", null, false);

                switch (cmd)
                {
                    case "q":
                        _RunForever = false;
                        break;
                    case "?":
                        Menu();
                        break;
                    case "cls":
                        Console.Clear();
                        break;
                    case "query":
                        CursorQuery();
                        break;
                    case "query edge":
                        CursorQueryEdge();
                        break;
                    case "query vertex":
                        CursorQueryVertex();
                        break;

                    case "list db":
                        ListDatabases();
                        break;
                    case "add db":
                        AddDatabase();
                        break;
                    case "del db":
                        DeleteDatabase();
                        break;
                    case "db exists":
                        DatabaseExists();
                        break;

                    case "list users":
                        ListUsers();
                        break;
                    case "get user":
                        GetUser();
                        break;
                    case "add user":
                        AddUser();
                        break;
                    case "del user":
                        DeleteUser();
                        break;
                    case "user exists":
                        UserExists();
                        break;

                    case "list coll":
                        ListCollections();
                        break;
                    case "get coll":
                        GetCollection();
                        break;
                    case "add coll":
                        AddCollection();
                        break;
                    case "del coll":
                        DeleteCollection();
                        break;
                    case "coll exists":
                        CollectionExists();
                        break;

                    case "list graph":
                        ListGraphs();
                        break;
                    case "get graph":
                        GetGraph();
                        break;
                    case "add graph":
                        AddGraph();
                        break;
                    case "add graph edge":
                        AddGraphEdge();
                        break;
                    case "del graph":
                        DeleteGraph();
                        break;
                    case "graph exists":
                        GraphExists();
                        break;

                    case "list vertices":
                        ListVertices();
                        break;
                    case "add vertex":
                        AddVertex();
                        break;
                    case "update vertex":
                        UpdateVertex();
                        break;
                    case "replace vertex":
                        ReplaceVertex();
                        break;
                    case "get vertex":
                        GetVertex();
                        break;
                    case "first vertex":
                        FirstVertex();
                        break;
                    case "del vertex":
                        DeleteVertex();
                        break;

                    case "list edges":
                        ListEdges();
                        break;
                    case "add edge":
                        AddEdge();
                        break;
                    case "update edge":
                        UpdateEdge();
                        break;
                    case "replace edge":
                        ReplaceEdge();
                        break;
                    case "get edge":
                        GetEdge();
                        break;
                    case "first edge":
                        FirstEdge();
                        break;
                    case "del edge":
                        DeleteEdge();
                        break;
                }
            }

            Console.WriteLine("Hello World!");
        }

        static void Menu()
        {
            Console.WriteLine("General commands");
            Console.WriteLine("  q                quit this program");
            Console.WriteLine("  ?                help, i.e. this menu");
            Console.WriteLine("  cls              clear the screen");
            Console.WriteLine("  query            execute cursor query");
            Console.WriteLine("");
            Console.WriteLine("Database commands");
            Console.WriteLine("  list db          list databases");
            Console.WriteLine("  add db           add a database");
            Console.WriteLine("  del db           delete a database");
            Console.WriteLine("  db exists        check if a database exists by name");
            Console.WriteLine("");
            Console.WriteLine("User commands");
            Console.WriteLine("  list users       list users");
            Console.WriteLine("  get user         get a user");
            Console.WriteLine("  add user         add a user");
            Console.WriteLine("  del user         delete a user");
            Console.WriteLine("  user exists      check if a user exists by name");
            Console.WriteLine("");
            Console.WriteLine("Collection commands");
            Console.WriteLine("  list coll        list collections");
            Console.WriteLine("  get coll         get a collection");
            Console.WriteLine("  add coll         add a collection");
            Console.WriteLine("  del coll         delete a collection");
            Console.WriteLine("  coll exists      check if a collection exists by name");
            Console.WriteLine("");
            Console.WriteLine("Graph commands");
            Console.WriteLine("  list graph       list graphs");
            Console.WriteLine("  get graph        get a graph");
            Console.WriteLine("  add graph        add a graph");
            Console.WriteLine("  add graph edge   add a graph edge collection");
            Console.WriteLine("  del graph        delete a graph");
            Console.WriteLine("  graph exists     check if a graph exists by name");
            Console.WriteLine("");
            Console.WriteLine("Vertex commands");
            Console.WriteLine("  list vertices    list vertices");
            Console.WriteLine("  add vertex       add a vertex");
            Console.WriteLine("  update vertex    update a vertex");
            Console.WriteLine("  replace vertex   replace a vertex");
            Console.WriteLine("  get vertex       get a vertex");
            Console.WriteLine("  first vertex     get the first matching vertex");
            Console.WriteLine("  del vertex       delete a vertex");
            Console.WriteLine("");
            Console.WriteLine("Edge commands");
            Console.WriteLine("  list edges       list edges");
            Console.WriteLine("  add edge         add an edge");
            Console.WriteLine("  update edge      update an edge");
            Console.WriteLine("  replace edge     replace an edge");
            Console.WriteLine("  get edge         get an edge");
            Console.WriteLine("  first edge       get the first matching edge");
            Console.WriteLine("  del edge         delete an edge");
            Console.WriteLine("");
        }
         
        static void CursorQuery()
        {
            string query = InputString("Query:", null, true);
            if (String.IsNullOrEmpty(query)) return;

            AdbResult result = _Client.ExecuteCursorQuery<dynamic>(query).Result;
            Console.WriteLine(result.ToJson(true)); 
        }

        static void CursorQueryEdge()
        {
            AdbResult result = _Client.ExecuteCursorQuery<AdbEdge>("FOR d IN edges RETURN d").Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void CursorQueryVertex()
        {
            AdbResult result = _Client.ExecuteCursorQuery<AdbVertex>("FOR d IN startVertices RETURN d").Result;
            Console.WriteLine(result.ToJson(true));
        }

        static string SerializeJson(object obj, bool pretty)
        {
            if (obj == null) return null;
            string json;

            if (pretty)
            {
                json = JsonConvert.SerializeObject(
                  obj,
                  Newtonsoft.Json.Formatting.Indented,
                  new JsonSerializerSettings
                  {
                      NullValueHandling = NullValueHandling.Ignore,
                      DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                  });
            }
            else
            {
                json = JsonConvert.SerializeObject(obj,
                  new JsonSerializerSettings
                  {
                      NullValueHandling = NullValueHandling.Ignore,
                      DateTimeZoneHandling = DateTimeZoneHandling.Utc
                  });
            }

            return json;
        }

        #region Database

        static void ListDatabases()
        {
            AdbResult result = _Client.ListDatabases().Result; 
            Console.WriteLine(result.ToJson(true));
        }

        static void AddDatabase()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;
            List<AdbUser> users = InputUsers();
            AdbDatabaseOptions options = InputDatabaseOptions();
            AdbResult result = _Client.CreateDatabase(name, users, options).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void DeleteDatabase()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;
            AdbResult result = _Client.DeleteDatabase(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void DatabaseExists()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbResult result = _Client.DatabaseExists(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        #endregion

        #region User

        static void ListUsers()
        {
            AdbResult result = _Client.ListUsers().Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void GetUser()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbResult result = _Client.GetUser(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void UserExists()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbResult result = _Client.UserExists(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void AddUser()
        {
            AdbUser user = InputUser();
            if (user == null) return;

            AdbResult result = _Client.CreateUser(user).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void DeleteUser()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbResult result = _Client.DeleteUser(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        #endregion

        #region Collection

        static void ListCollections()
        {
            AdbResult result = _Client.ListCollections().Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void GetCollection()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbResult result = _Client.GetCollection(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void CollectionExists()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbResult result = _Client.CollectionExists(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void AddCollection()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbResult result = _Client.CreateCollection(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void DeleteCollection()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbResult result = _Client.DeleteCollection(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        #endregion

        #region Graph

        static void ListGraphs()
        {
            AdbResult result = _Client.ListGraphs().Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void GetGraph()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbResult result = _Client.GetGraph(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void AddGraph()
        {
            AdbGraph graph = InputGraph();
            if (graph == null) return;

            AdbResult result = _Client.CreateGraph(graph).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void AddGraphEdge()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbEdgeDefinition definition = InputEdgeDefinition();
            if (definition == null) return;

            AdbResult result = _Client.AddGraphEdgeCollection(name, definition).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void DeleteGraph()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbResult result = _Client.DeleteGraph(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void GraphExists()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return;

            AdbResult result = _Client.GraphExists(name).Result;
            Console.WriteLine(result.ToJson(true));
        }

        #endregion

        #region Vertex

        static void ListVertices()
        {
            string colName = InputString("Collection name:", null, true);
            if (String.IsNullOrEmpty(colName)) return;

            Dictionary<string, string> filter = InputDictionaryStringString();

            AdbResult result = _Client.ListVertices(colName, filter).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void AddVertex()
        {
            string graphName = InputString("Graph name:", null, true);
            if (String.IsNullOrEmpty(graphName)) return;
            
            string colName = InputString("Collection name:", null, true);
            Dictionary<string, object> data = InputDictionaryStringObject();

            AdbResult result = _Client.CreateVertex(graphName, colName, data.ToJson(true)).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void UpdateVertex()
        {
            string graphName = InputString("Graph name:", null, true);
            if (String.IsNullOrEmpty(graphName)) return;

            string colName = InputString("Collection name:", null, true);
            string key = InputString("Key:", null, true);
            Dictionary<string, object> data = InputDictionaryStringObject();

            AdbResult result = _Client.UpdateVertex(graphName, colName, key, data.ToJson(true)).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void ReplaceVertex()
        {
            string graphName = InputString("Graph name:", null, true);
            if (String.IsNullOrEmpty(graphName)) return;

            string colName = InputString("Collection name:", null, true);
            string key = InputString("Key:", null, true);
            Dictionary<string, object> data = InputDictionaryStringObject();

            AdbResult result = _Client.ReplaceVertex(graphName, colName, key, data.ToJson(true)).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void GetVertex()
        {
            string graphName = InputString("Graph name:", null, true);
            if (String.IsNullOrEmpty(graphName)) return;

            string colName = InputString("Collection name:", null, true);
            string key = InputString("Key:", null, true);

            AdbResult result = _Client.GetVertex(graphName, colName, key).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void FirstVertex()
        {
            string colName = InputString("Collection name:", null, true);
            if (String.IsNullOrEmpty(colName)) return;

            Dictionary<string, string> filter = InputDictionaryStringString();

            AdbResult result = _Client.FirstVertex(colName, filter).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void DeleteVertex()
        { 
            string graphName = InputString("Graph name:", null, true);
            if (String.IsNullOrEmpty(graphName)) return;

            string colName = InputString("Collection name:", null, true);
            string key = InputString("Key:", null, true);
            
            AdbResult result = _Client.DeleteVertex(graphName, colName, key).Result;
            Console.WriteLine(result);
        }

        #endregion

        #region Edge

        static void ListEdges()
        {
            string colName = InputString("Collection name:", null, true);
            if (String.IsNullOrEmpty(colName)) return;

            Dictionary<string, string> filter = InputDictionaryStringString();

            AdbResult result = _Client.ListEdges(colName, filter).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void AddEdge()
        {
            string graphName = InputString("Graph name:", null, true);
            if (String.IsNullOrEmpty(graphName)) return;

            string colName = InputString("Collection name:", null, false);
            string fromId = InputString("From ID:", null, false);
            string toId = InputString("To ID:", null, false);
            Dictionary<string, object> data = InputDictionaryStringObject();

            AdbResult result = _Client.CreateEdge(graphName, colName, fromId, toId, data.ToJson(true)).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void UpdateEdge()
        {
            string graphName = InputString("Graph name:", null, true);
            if (String.IsNullOrEmpty(graphName)) return;

            string colName = InputString("Collection name:", null, true);
            string key = InputString("Key:", null, true);
            Dictionary<string, object> data = InputDictionaryStringObject();

            AdbResult result = _Client.UpdateEdge(graphName, colName, key, data.ToJson(true)).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void ReplaceEdge()
        {
            string graphName = InputString("Graph name:", null, true);
            if (String.IsNullOrEmpty(graphName)) return;

            string colName = InputString("Collection name:", null, true);
            string key = InputString("Key:", null, true);
            Dictionary<string, object> data = InputDictionaryStringObject();

            AdbResult result = _Client.ReplaceEdge(graphName, colName, key, data.ToJson(true)).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void GetEdge()
        {
            string graphName = InputString("Graph name:", null, true);
            if (String.IsNullOrEmpty(graphName)) return;

            string colName = InputString("Collection name:", null, true);
            string key = InputString("Key:", null, true);

            AdbResult result = _Client.GetEdge(graphName, colName, key).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void FirstEdge()
        {
            string colName = InputString("Collection name:", null, true);
            if (String.IsNullOrEmpty(colName)) return;

            Dictionary<string, string> filter = InputDictionaryStringString();

            AdbResult result = _Client.FirstEdge(colName, filter).Result;
            Console.WriteLine(result.ToJson(true));
        }

        static void DeleteEdge()
        {
            string graphName = InputString("Graph name:", null, true);
            if (String.IsNullOrEmpty(graphName)) return;

            string colName = InputString("Collection name:", null, true);
            string key = InputString("Key:", null, true);

            AdbResult result = _Client.DeleteEdge(graphName, colName, key).Result;
            Console.WriteLine(result.ToJson(true));
        }

        #endregion

        #region Input

        static bool InputBoolean(string question, bool yesDefault)
        {
            Console.Write(question);

            if (yesDefault) Console.Write(" [Y/n]? ");
            else Console.Write(" [y/N]? ");

            string userInput = Console.ReadLine();

            if (String.IsNullOrEmpty(userInput))
            {
                if (yesDefault) return true;
                return false;
            }

            userInput = userInput.ToLower();

            if (yesDefault)
            {
                if (
                    (String.Compare(userInput, "n") == 0)
                    || (String.Compare(userInput, "no") == 0)
                   )
                {
                    return false;
                }

                return true;
            }
            else
            {
                if (
                    (String.Compare(userInput, "y") == 0)
                    || (String.Compare(userInput, "yes") == 0)
                   )
                {
                    return true;
                }

                return false;
            }
        }

        static string InputString(string question, string defaultAnswer, bool allowNull)
        {
            while (true)
            {
                Console.Write(question);

                if (!String.IsNullOrEmpty(defaultAnswer))
                {
                    Console.Write(" [" + defaultAnswer + "]");
                }

                Console.Write(" ");

                string userInput = Console.ReadLine();

                if (String.IsNullOrEmpty(userInput))
                {
                    if (!String.IsNullOrEmpty(defaultAnswer)) return defaultAnswer;
                    if (allowNull) return null;
                    else continue;
                }

                return userInput;
            }
        }

        static List<string> InputStringList(string question)
        {
            List<string> ret = new List<string>();

            while (true)
            {
                Console.Write(question); 
                Console.Write(" "); 
                string userInput = Console.ReadLine(); 
                if (String.IsNullOrEmpty(userInput)) break;
                ret.Add(userInput);
            }

            return ret;
        }

        static int InputInteger(string question, int defaultAnswer, bool positiveOnly, bool allowZero)
        {
            while (true)
            {
                Console.Write(question);
                Console.Write(" [" + defaultAnswer + "] ");

                string userInput = Console.ReadLine();

                if (String.IsNullOrEmpty(userInput))
                {
                    return defaultAnswer;
                }

                int ret = 0;
                if (!Int32.TryParse(userInput, out ret))
                {
                    Console.WriteLine("Please enter a valid integer.");
                    continue;
                }

                if (ret == 0)
                {
                    if (allowZero)
                    {
                        return 0;
                    }
                }

                if (ret < 0)
                {
                    if (positiveOnly)
                    {
                        Console.WriteLine("Please enter a value greater than zero.");
                        continue;
                    }
                }

                return ret;
            }
        }

        static decimal InputDecimal(string question, decimal defaultAnswer, bool positiveOnly, bool allowZero)
        {
            while (true)
            {
                Console.Write(question);
                Console.Write(" [" + defaultAnswer + "] ");

                string userInput = Console.ReadLine();

                if (String.IsNullOrEmpty(userInput))
                {
                    return defaultAnswer;
                }

                decimal ret = 0;
                if (!Decimal.TryParse(userInput, out ret))
                {
                    Console.WriteLine("Please enter a valid decimal.");
                    continue;
                }

                if (ret == 0)
                {
                    if (allowZero)
                    {
                        return 0;
                    }
                }

                if (ret < 0)
                {
                    if (positiveOnly)
                    {
                        Console.WriteLine("Please enter a value greater than zero.");
                        continue;
                    }
                }

                return ret;
            }
        }

        static Dictionary<string, object> InputDictionaryStringObject()
        {
            Console.WriteLine("Build dictionary, press ENTER on 'Key' to exit");

            Dictionary<string, object> ret = new Dictionary<string, object>();

            while (true)
            {
                Console.Write("Key   : ");
                string key = Console.ReadLine();
                if (String.IsNullOrEmpty(key)) return ret;

                Console.Write("Value : ");
                string val = Console.ReadLine();
                ret.Add(key, val);
            }
        }

        static Dictionary<string, string> InputDictionaryStringString()
        {
            Console.WriteLine("Build dictionary, press ENTER on 'Key' to exit");

            Dictionary<string, string> ret = new Dictionary<string, string>();

            while (true)
            {
                Console.Write("Key   : ");
                string key = Console.ReadLine();
                if (String.IsNullOrEmpty(key)) return ret;

                Console.Write("Value : ");
                string val = Console.ReadLine();
                ret.Add(key, val);
            }
        }

        static AdbUser InputUser()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return null;
            string pass = InputString("Password:", null, true);
            Console.WriteLine("User metadata");
            Dictionary<string, object> extra = InputDictionaryStringObject();
            return new AdbUser(name, pass, true, extra);
        }

        static List<AdbUser> InputUsers()
        {
            Console.WriteLine("Build user list, press ENTER on 'Name' to exit");

            List<AdbUser> ret = new List<AdbUser>();

            while (true)
            {
                string name = InputString("Name:", null, true);
                if (String.IsNullOrEmpty(name)) break;
                string pass = InputString("Password:", null, true);
                Console.WriteLine("User metadata");
                Dictionary<string, object> extra = InputDictionaryStringObject();
                AdbUser u = new AdbUser(name, pass, true, extra);
                ret.Add(u);
            }

            return ret;
        }

        static AdbDatabaseOptions InputDatabaseOptions()
        {
            AdbDatabaseOptions ret = new AdbDatabaseOptions(
                InputString("Sharding:", "single", false),
                InputString("Replication factor:", "1", false),
                InputInteger("Write concern:", 1, true, false));
            return ret;
        }

        static AdbGraph InputGraph()
        {
            string name = InputString("Name:", null, true);
            if (String.IsNullOrEmpty(name)) return null;

            AdbGraph graph = new AdbGraph(name, InputEdgeDefinitions());
            return graph;
        }

        static AdbEdgeDefinition InputEdgeDefinition()
        {
            string name = InputString("Edge collection:", null, true);
            if (String.IsNullOrEmpty(name)) return null;

            AdbEdgeDefinition ret = new AdbEdgeDefinition();
            ret.CollectionName = name;

            Console.WriteLine("From vertex collections, press ENTER to end");
            ret.FromVertexCollections = InputStringList("From:");
            Console.WriteLine("To vertex collections, press ENTER to end");
            ret.ToVertexCollections = InputStringList("To:");

            return ret;
        }

        static List<AdbEdgeDefinition> InputEdgeDefinitions()
        {
            List<AdbEdgeDefinition> ret = new List<AdbEdgeDefinition>();
            
            while (true)
            {
                AdbEdgeDefinition definition = InputEdgeDefinition();
                if (definition == null) break;
                ret.Add(definition);
            }

            return ret;
        }

        #endregion
    }
}
