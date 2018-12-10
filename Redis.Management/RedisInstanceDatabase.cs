using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Management
{
    public class RedisInstanceDatabase
    {
        public RedisInstance RedisInstance { get; }

        public int Index { get; set; }
        public int Keys { get; set; }
        public int Expires { get; set; }
        public int AvgTTL { get; set; }

        public RedisInstanceDatabase(RedisInstance redisInstance)
        {
            RedisInstance = redisInstance;
        }
    }
}
