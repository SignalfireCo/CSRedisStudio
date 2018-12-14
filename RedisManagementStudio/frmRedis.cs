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
    public partial class frmRedis : DockContent
    {
        private RedisInstance _instance;

        public frmRedis()
        {
            InitializeComponent();
        }

        public frmRedis(RedisInstance instance) : this()
        {
            _instance = instance;
            if (_instance != null)
            {
                this.DockHandler.TabText = string.Format("{0}:{1}", _instance.IPEndPoint.Address, _instance.IPEndPoint.Port);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            _instance.Monitor.MonitorReceived += Monitor_MonitorReceived;
            _instance.Monitor.Start();

            bool isCancel = false;
            RMStudio.Instance.Publish(FocusKeys.REDIS_MONITOR, _instance, out isCancel);
        }

        private void Monitor_MonitorReceived(object sender, CSRedis.RedisMonitorEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private void tsbStop_Click(object sender, EventArgs e)
        {
            _instance.Monitor.MonitorReceived -= Monitor_MonitorReceived;
            _instance.Monitor.Stop();
        }
    }
}
