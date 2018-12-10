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
    public partial class uctDatabase : UserControl
    {
        public RedisInstanceDatabase Database { get; }
        public uctDatabase()
        {
            InitializeComponent();
        }
        public uctDatabase(RedisInstanceDatabase database)
            :this()
        {
            Database = database;
        }
    }
}
