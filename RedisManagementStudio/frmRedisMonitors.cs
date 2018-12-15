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

            tscbSources.SelectedIndex = 1;
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
                    switch (tscbSources.SelectedIndex)
                    {
                        case 1: //不显示心跳包
                            string[] strList = e.Message.ToString().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if(!strList[strList.Length - 1].Equals("\"PING\""))
                            {
                                listBox1.Items.Insert(0, e.Message);
                            } 
                            break;

                        default:
                            listBox1.Items.Insert(0, e.Message);
                            break;
                    }
                }
                ));
            }
            else
            {
                listBox1.Items.Insert(0, e.Message);
            }
        }

        private void tsbClear_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void tscbSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }
    }
}
