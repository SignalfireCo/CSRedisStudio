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
using System.Collections.Concurrent;

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

                foreach(string key in _instance.Infos.Servers.Keys)
                {
                    Console.WriteLine("{0}:{1}", key, _instance.Infos.Servers[key]);

                    ListViewItem item = listView1.Items.Add(key);
                    item.SubItems.Add(_instance.Infos.Servers[key]);

                }

                for (int i = 0; i < listView1.Columns.Count; i++)
                {
                    listView1.Columns[i].Width = -1;
                }

                foreach (string key in _instance.Infos.Stats.Keys)
                {
                    string value = string.Empty;
                    _instance.Infos.Stats.TryGetValue(key, out value);

                    ListViewItem item = lvStats.Items.Add(key);
                    item.SubItems.Add(value);
                }
                for (int i = 0; i < lvStats.Columns.Count; i++)
                {
                    lvStats.Columns[i].Width = -1;
                }


                foreach (string key in _instance.Infos.Persistence.Keys)
                {
                    ListViewItem item = lvPersistence.Items.Add(key);
                    item.SubItems.Add(_instance.Infos.Persistence[key]);
                }
                for (int i = 0; i < lvPersistence.Columns.Count; i++)
                {
                    lvPersistence.Columns[i].Width = -1;
                }



                foreach (string key in _instance.Infos.Memory.Keys)
                {
                    ListViewItem item = lvMemory.Items.Add(key);
                    item.SubItems.Add(_instance.Infos.Memory[key]);
                }
                for (int i = 0; i < lvMemory.Columns.Count; i++)
                {
                    lvMemory.Columns[i].Width = -1;
                }


                foreach (string key in _instance.Infos.Replication.Keys)
                {
                    ListViewItem item = lvReplication.Items.Add(key);
                    item.SubItems.Add(_instance.Infos.Replication[key]);
                }
                for (int i = 0; i < lvReplication.Columns.Count; i++)
                {
                    lvReplication.Columns[i].Width = -1;
                }


                foreach (string key in _instance.Configs.Keys)
                {
                    ListViewItem item = lvConfigs.Items.Add(key);
                    item.SubItems.Add(_instance.Configs[key]);
                }
                for (int i = 0; i < lvConfigs.Columns.Count; i++)
                {
                    lvConfigs.Columns[i].Width = -1;
                }

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

        private void frmRedis_Load(object sender, EventArgs e)
        {

        }
    }
}
