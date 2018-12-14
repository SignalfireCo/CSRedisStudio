using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Redis.Management;

namespace RedisManagementStudio
{
    public partial class frmDatabase : DockContent
    {
        private RedisInstanceDatabase _database;

        public frmDatabase()
        {
            InitializeComponent();
        }
        public frmDatabase(RedisInstanceDatabase database)
            :this()
        {
            _database = database;
        }

        private void frmDatabase_Load(object sender, EventArgs e)
        {
            if (_database != null)
            {
                this.TabText = string.Format("db{0} [{1}]", _database.Index, _database.RedisInstance.IPEndPoint);
                
            }
        }
    }
}
