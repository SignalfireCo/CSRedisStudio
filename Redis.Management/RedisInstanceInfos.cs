using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

namespace Redis.Management
{
    public class RedisInstanceInfos: ConcurrentDictionary<string, ConcurrentDictionary<string, string>>
    {
        public ConcurrentDictionary<string, string> Servers
        {
            get {
                if (!this.ContainsKey("Server"))
                {
                    this["Server"] = new ConcurrentDictionary<string, string>();
                }
                return this["Server"];
            }
        }
        public ConcurrentDictionary<string, string> Clients
        {
            get
            {
                if (!this.ContainsKey("Clients"))
                {
                    this["Clients"] = new ConcurrentDictionary<string, string>();
                }
                return this["Clients"];
            }
        }
        public ConcurrentDictionary<string, string> Memory
        {
            get
            {
                if (!this.ContainsKey("Memory"))
                {
                    this["Memory"] = new ConcurrentDictionary<string, string>();
                }
                return this["Memory"];
            }
        }
        public ConcurrentDictionary<string, string> Persistence
        {
            get
            {
                if (!this.ContainsKey("Persistence"))
                {
                    this["Persistence"] = new ConcurrentDictionary<string, string>();
                }
                return this["Persistence"];
            }
        }
        public ConcurrentDictionary<string, string> Stats
        {
            get
            {
                if (!this.ContainsKey("Stats"))
                {
                    this["Stats"] = new ConcurrentDictionary<string, string>();
                }
                return this["Stats"];
            }
        }
        public ConcurrentDictionary<string, string> Replication
        {
            get
            {
                if (!this.ContainsKey("Replication"))
                {
                    this["Replication"] = new ConcurrentDictionary<string, string>();
                }
                return this["Replication"];
            }
        }
        public ConcurrentDictionary<string, string> CPU
        {
            get
            {
                if (!this.ContainsKey("CPU"))
                {
                    this["CPU"] = new ConcurrentDictionary<string, string>();
                }
                return this["CPU"];
            }
        }
        public ConcurrentDictionary<string, string> Cluster
        {
            get
            {
                if (!this.ContainsKey("Cluster"))
                {
                    this["Cluster"] = new ConcurrentDictionary<string, string>();
                }
                return this["Cluster"];
            }
        }
        public ConcurrentDictionary<string, string> Keyspace
        {
            get
            {
                if (!this.ContainsKey("Keyspace"))
                {
                    this["Keyspace"] = new ConcurrentDictionary<string, string>();
                }
                return this["Keyspace"];
            }
        }
    }
}
