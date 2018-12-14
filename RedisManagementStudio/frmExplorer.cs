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
using System.Net;
using CSRedis;

namespace RedisManagementStudio
{
    public partial class frmExplorer : DockContent
    {
        private RedisInstance _instance;

        public frmExplorer()
        {
            InitializeComponent();

            this.TabText = "资源管理器";
            RMStudio.Instance.RedisInstanceConnected += Instance_RedisInstanceConnected;
        }

        private void Instance_RedisInstanceConnected(object sender, TEventArgs<Redis.Management.RedisInstance> e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => { Instance_RedisInstanceConnected(sender, e); }));
            }
            else
            {
                _instance = e.Item;
                string txtName = string.Format("{0}:{1}", e.Item.IPEndPoint.Address, e.Item.IPEndPoint.Port);

                TreeNode node = treeView1.Nodes.Add(txtName, txtName, "redis");

                TreeNode dbfolderNode = node.Nodes.Add("Databases", "Databases", "folder", "folder");
                node.Tag = e.Item;
                foreach (RedisInstanceDatabase db in e.Item.Databases)
                {
                    string key = string.Format("db{0}", db.Index);
                    string Text = string.Format("db{0}[{1}]", db.Index, db.Keys);
                    TreeNode dbNode = dbfolderNode.Nodes.Add(key, Text, db.Keys > 0 ? "database" : "database-off", "database-selected");
                    dbNode.Tag = db;
                }
                dbfolderNode.Expand();



                node.Expand();
            }

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmConnect connect = new frmConnect();
            connect.ShowDialog(this);
        }


        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            bool isCancelled = true;
            if (e.Node.Tag != null)
            {
                RMStudio.Instance.Publish(FocusKeys.STUDIO_FOCUS, e.Node.Tag, out isCancelled);
            }
            else
            {
                RMStudio.Instance.Publish(FocusKeys.STUDIO_FOCUS, e.Node.Text, out isCancelled, _instance);
            }
            e.Cancel = isCancelled;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tsbDisconnect.Enabled = false;

            if(e.Node.Tag!=null && e.Node.Tag.GetType() == typeof(RedisInstance))
            {
                tsbDisconnect.Enabled = true;
            }
        }
    }
}
