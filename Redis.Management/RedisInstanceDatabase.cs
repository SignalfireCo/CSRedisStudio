using System;
using System.Collections.Generic;
using System.Text;
using CSRedis;

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

        public List<string[]> Execute(string script)
        {
            List<string[]> result = new List<string[]>();

            RedisClient client = RedisInstance.GetRedisClient();

            client.Select(Index);

            RedisScan<string> scan = client.Scan(0, null, 1000);

            if (scan.Items.Length > 0)
            {
                string[] values = client.MGet(scan.Items);

                client.TransactionQueued += Client_TransactionQueued;
                //client.StartPipeTransaction();
                client.Multi();
                foreach (string key in scan.Items)
                {
                    string[] newItem = new string[3];
                    newItem[0] = key;
                    result.Add(newItem);

                    client.Type(key);
                }
                object[] objs = client.Exec();

                for (int i = 0; i < result.Count; i++)
                {
                    result[i][2] = values[i] == null ? "<Nil>" : values[i];
                    result[i][1] = objs[i].ToString();
                }
            }

            return result;
        }

        private void Client_TransactionQueued(object sender, RedisTransactionQueuedEventArgs e)
        {
            //Console.WriteLine(e.Arguments)
        }
    }
}
