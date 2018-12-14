using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.Concurrent;
using CSRedis;
using System.Threading;

namespace Redis.Management
{
    public class RedisInstance
    {
        private RedisClient _pulseClient;
        private CSRedisClient _client;
        private Thread _pulseThread;

        public int PulseInterval { get; set; }
        public IPEndPoint IPEndPoint { get; set; }
        public string Password { get; set; }
        private CancellationTokenSource _cancellationSource;

        private Dictionary<string, string> _configs = new Dictionary<string, string>();
        public Dictionary<string,string> Configs
        {
            get
            {
                return _configs;
            }
        }

        private List<RedisInstanceDatabase> _databases = new List<RedisInstanceDatabase>();
        public List<RedisInstanceDatabase> Databases
        {
            get { return _databases; }
        }

        private RedisInstanceInfos _infos = new RedisInstanceInfos();
        public RedisInstanceInfos Infos
        {
            get
            {
                return _infos;
            }
        }


        private RedisInstanceStatus _status = RedisInstanceStatus.Disconnected;
        public RedisInstanceStatus Status
        {
            get { return _status; }
            protected set {
                if (_status.Equals(value)) return;

                RedisInstanceStatus original = _status;
                _status = value;
                this.OnStatusChanged(new TargetChangedEventArgs<RedisInstanceStatus>(original, value));
            }
        }

        public event EventHandler<TargetChangedEventArgs<RedisInstanceStatus>> StatusChanged;
        protected virtual void OnStatusChanged(TargetChangedEventArgs<RedisInstanceStatus> args)
        {
            StatusChanged?.Invoke(this, args);
        }

        private RedisInstanceMonitor _monitor;
        public RedisInstanceMonitor Monitor
        {
            get
            {
                if (_monitor == null)
                {
                    _monitor = new RedisInstanceMonitor(this);
                }
                return _monitor;
            }
        }

        public RedisInstance()
        {
            PulseInterval = 3000;

        }

        public bool Connect(IPEndPoint endpoint, string password)
        {
            if (_status.Equals(RedisInstanceStatus.Connected)) throw new Exception("Instance is already connected to a server.");

            IPEndPoint = endpoint;
            Password = password;
            string connString = string.Format("{0}[:{1}],password={2},defaultDatabase=0,poolsize=50,ssl=false,writeBuffer=10240,prefix=", endpoint.Address, endpoint.Port, password);
            _client = new CSRedisClient(connString);

            _pulseClient = new RedisClient(endpoint);
            bool isConnected = false;
            
            isConnected = _pulseClient.Connect(3000);
            
            if (isConnected)
            {
                string authRtn = _pulseClient.Auth(password);

                if (authRtn.ToLower().Equals("ok"))
                {
                    Tuple<string,string>[] rtn = _pulseClient.ConfigGet("*");
                    foreach(Tuple<string,string> tuple in rtn)
                    {
                        _configs[tuple.Item1] = tuple.Item2;
                        Console.WriteLine("{0}={1}", tuple.Item1, tuple.Item2);
                    }

                    _databases.Clear();
                    if (_configs.ContainsKey("databases"))
                    {
                        int dbCount = 0;

                        if(int.TryParse(_configs["databases"], out dbCount))
                        {
                            for(int i = 0; i < dbCount; i++)
                            {
                                RedisInstanceDatabase db = new RedisInstanceDatabase(this) {
                                    Index = i,
                                };
                                _databases.Add(db);
                            }
                        }
                    }
                    

                    string info = _pulseClient.Info();
                    Console.WriteLine(info);

                    string section = string.Empty;
                    string[] infoItems = info.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    foreach(string item in infoItems)
                    {
                        string[] keyvalue = item.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                        if(keyvalue.Length == 1)
                        {
                            section = keyvalue[0].Substring(2, keyvalue[0].Length - 2);
                        }
                        else if(keyvalue.Length == 2)
                        {
                            switch (section.ToLower())
                            {
                                case "server":
                                    this.Infos.Servers[keyvalue[0]] = keyvalue[1].Trim();
                                    break;
                                case "clients":
                                    this.Infos.Clients[keyvalue[0]] = keyvalue[1].Trim();
                                    break;
                                case "memory":
                                    this.Infos.Memory[keyvalue[0]] = keyvalue[1].Trim();
                                    break;
                                case "persistence":
                                    this.Infos.Persistence[keyvalue[0]] = keyvalue[1].Trim();
                                    break;
                                case "stats":
                                    this.Infos.Stats[keyvalue[0]] = keyvalue[1].Trim();
                                    break;
                                case "replication":
                                    this.Infos.Replication[keyvalue[0]] = keyvalue[1].Trim();
                                    break;
                                case "cpu":
                                    this.Infos.CPU[keyvalue[0]] = keyvalue[1].Trim();
                                    break;
                                case "cluster":
                                    this.Infos.Cluster[keyvalue[0]] = keyvalue[1].Trim();
                                    break;
                                case "keyspace":
                                    this.Infos.Keyspace[keyvalue[0]] = keyvalue[1].Trim();
                                    int dbIndex = -1;
                                    if(int.TryParse(keyvalue[0].Substring(keyvalue[0].Length -1, 1), out dbIndex)){
                                        RedisInstanceDatabase db = _databases[dbIndex];
                                        string[] valueStrings = keyvalue[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                        foreach(string valueString in valueStrings)
                                        {
                                            string[] kv = valueString.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                                            switch (kv[0])
                                            {
                                                case "keys":
                                                    int keysValue = 0;
                                                    if (int.TryParse(kv[1], out keysValue))
                                                    {
                                                        db.Keys = keysValue;
                                                    }
                                                    break;
                                                case "expires":
                                                    int expiresValue = 0;
                                                    if (int.TryParse(kv[1], out expiresValue))
                                                    {
                                                        db.Expires = expiresValue;
                                                    }
                                                    break;
                                                case "avg_ttl":
                                                    int avgTTL = 0;
                                                    if (int.TryParse(kv[1], out avgTTL))
                                                    {
                                                        db.AvgTTL = avgTTL;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    break;

                                default:
                                    break;
                            }

                        }
                    }

                    if (_cancellationSource != null)
                    {
                        _cancellationSource.Cancel();
                    }
                    _pulseThread = new Thread(new ThreadStart(PulseThread));
                    _pulseThread.IsBackground = true;
                    _pulseThread.Start();

                    this.Status = RedisInstanceStatus.Connected;
                }
            }

            return isConnected;
        }

        public void ConnectAsync()
        {

        }

        public void Disconnect()
        {


            if (_cancellationSource != null)
            {
                _cancellationSource.Cancel();
            }

            Status = RedisInstanceStatus.Disconnected;

        }

        private void PulseThread()
        {
            _cancellationSource = new CancellationTokenSource();

            while (!_cancellationSource.Token.IsCancellationRequested)
            {
                try
                {
                    string pingRtn = _pulseClient.Ping();                    
                }
                catch (Exception)
                {
                    Status = RedisInstanceStatus.Disconnected;
                    break;
                }

                Thread.Sleep(PulseInterval);
            }

            _pulseClient.Dispose();
            _cancellationSource = null;
        }
    }

    public enum RedisInstanceStatus
    {
        Disconnected = 0
            , Connected = 1
    }
}
