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
    public partial class frmRedisMonitors : DockContent
    {
        private RedisInstance _instance;

        public frmRedisMonitors()
        {
            InitializeComponent();
        }

        public frmRedisMonitors(RedisInstance instance) : this()
        {
            _instance = instance;
            if (_instance != null)
            {
                this.DockHandler.TabText = string.Format("服务器监控:{0}", _instance.IPEndPoint);
                _instance.Monitor.MonitorReceived += Monitor_MonitorReceived;
            }
        }

        private void Monitor_MonitorReceived(object sender, CSRedis.RedisMonitorEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    listBox1.Items.Insert(0, e.Message);
                }
                ));
            }
            else
            {
                listBox1.Items.Insert(0, e.Message);
            }
        }
    }
}
