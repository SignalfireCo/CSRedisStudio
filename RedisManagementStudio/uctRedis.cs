using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Redis.Management;

namespace RedisManagementStudio
{
    public partial class uctRedis : UserControl
    {
        public RedisInstance RedisInstance { get; }

        public uctRedis()
        {
            InitializeComponent();
        }
        public uctRedis(RedisInstance redisInstance)
            :this()
        {
            RedisInstance = redisInstance;
        }

        private void uctRedis_Load(object sender, EventArgs e)
        {

        }
    }
}
