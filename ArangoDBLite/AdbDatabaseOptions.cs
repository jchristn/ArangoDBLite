using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ArangoDBLite
{
    /// <summary>
    /// ArangoDB database options.
    /// </summary>
    public class AdbDatabaseOptions
    {
        #region Public-Members

        /// <summary>
        /// Sharding mode.
        /// </summary>
        [JsonProperty("sharding")]
        public string Sharding
        {
            get
            {
                return _Sharding;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    List<string> valid = new List<string> { null, "", "flexible", "single" };
                    if (!valid.Contains(value)) throw new ArgumentException("Invalid value for 'Sharding'.  Use one of null, '', 'flexible', or 'single'."); 
                }

                _Sharding = value;
            }
        }

        /// <summary>
        /// Replication factor.
        /// Valid values are either "satellite" or a positive integer.
        /// </summary>
        [JsonProperty("replicationFactor")]
        public string ReplicationFactor
        {
            get
            {
                return _ReplicationFactor;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    int testInt = 1;
                    if (!value.Equals("satellite") && !Int32.TryParse(value, out testInt))
                        throw new ArgumentException("ReplicationFactor must either be 'satellite' or a positive integer.");

                    if (testInt < 1) throw new ArgumentException("ReplicationFactor must either be 'satellite' or a positive integer.");
                }

                _ReplicationFactor = value;
            }
        }

        /// <summary>
        /// Write concern.
        /// 
        /// </summary>
        [JsonProperty("writeConcern")]
        public int? WriteConcern
        {
            get
            {
                return _WriteConcern;
            }
            set
            {
                if (value != null)
                {
                    if (value.Value < 1) throw new ArgumentException("WriteConcern must either be null or a positive integer.");
                    int testInt = 1;
                    if (Int32.TryParse(_ReplicationFactor, out testInt))
                    {
                        if (value > testInt) throw new ArgumentException("WriteConcern must not be larger than ReplicationFactor.");
                    }
                }
                
                _WriteConcern = value;
            }
        }

        #endregion

        #region Private-Members

        private string _Sharding = null;
        private string _ReplicationFactor = null;
        private int? _WriteConcern = null;

        #endregion

        #region Constructors-and-Factories

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public AdbDatabaseOptions()
        {

        }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="sharding">Sharding mode.</param>
        /// <param name="replicationFactor">Replication factor.</param>
        /// <param name="writeConcern">Write concern.</param>
        public AdbDatabaseOptions(string sharding = "single", string replicationFactor = null, int? writeConcern = null)
        {
            Sharding = sharding;
            ReplicationFactor = replicationFactor;
            WriteConcern = writeConcern;
        }

        #endregion

        #region Public-Methods

        #endregion

        #region Private-Methods

        #endregion
    }
}
