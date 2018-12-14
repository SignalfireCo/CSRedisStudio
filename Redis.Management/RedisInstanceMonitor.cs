using System;
using System.Collections.Generic;
using System.Text;
using CSRedis;
using System.Threading;

namespace Redis.Management
{
    public class RedisInstanceMonitor
    {
        public event EventHandler<RedisMonitorEventArgs> MonitorReceived;

        private RedisClient _redisClient;

        private RedisInstance _instance;
        public RedisInstance Instance
        {
            get { return _instance; }
        }


        public RedisInstanceMonitor(RedisInstance instance)
        {
            if (instance != null)
            {
                _instance = instance;
                _redisClient = new RedisClient(instance.IPEndPoint);
            }
        }

        public void Start()
        {
            Thread thread = new Thread(new ThreadStart(Monitoring));
            thread.IsBackground = true;
            thread.Start();
        }

        private void Monitoring()
        {
            string authRtn = _redisClient.Auth(_instance.Password);

            _redisClient.MonitorReceived += _redisClient_MonitorReceived;
            string rtn = _redisClient.Monitor();
            if (rtn.Equals("OK"))
            {
                
            }
        }

        private void _redisClient_MonitorReceived(object sender, RedisMonitorEventArgs e)
        {
            this.MonitorReceived?.Invoke(this, e);
        }

        public void Stop()
        {
            _redisClient.Quit();
            _redisClient.MonitorReceived -= _redisClient_MonitorReceived;
        }
    }
}
