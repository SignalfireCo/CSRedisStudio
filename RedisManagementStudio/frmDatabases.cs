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
    public partial class frmDatabases : DockContent
    {
        private RedisInstance _instance;

        public frmDatabases()
        {
            InitializeComponent();
        }

        public frmDatabases(RedisInstance instance) : this()
        {
            _instance = instance;
            if (_instance != null)
            {
                this.TabText = string.Format("Databases [{0}]", _instance.IPEndPoint);
            }
        }
    }
}
