using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Redis.Management;
using System.Net;

namespace RedisManagementStudio
{
    public partial class MainForm : Form
    {
        private const string TREEVIEW_FOCUS_CHANGE = "TREEVIEW_FOCUS_CHANGE";
        private RedisInstance _instance;

        public MainForm()
        {
            InitializeComponent();

            RMStudio.Instance.RedisInstanceConnected += Instance_RedisInstanceConnected;
            RMStudio.Instance.Subscribe(TREEVIEW_FOCUS_CHANGE, new Action<string, TargetChangingEventArgs<object>>(TreeView_Focus_Changing),  new Action<string, TargetChangedEventArgs<object>>(TreeView_Focus_Changed));
        }

        private void TreeView_Focus_Changing(string key, TargetChangingEventArgs<object> args)
        {
            if (MessageBox.Show("是否取消？", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                args.Cancel = true;
            }
        }

        private void TreeView_Focus_Changed(string key, TargetChangedEventArgs<object> args)
        {
            Console.WriteLine(args.NewTarget.ToString());
            Control control = null;
            if(args.NewTarget is RedisInstance)
            {
                control = new uctRedis((RedisInstance)args.NewTarget);
            }

            if (args.NewTarget is RedisInstanceDatabase)
            {
                control = new uctDatabase((RedisInstanceDatabase)args.NewTarget);
            }

            splitContainer1.Panel2.Controls.Clear();

            if (control != null)
            {
                control.Dock = DockStyle.Fill;
                splitContainer1.Panel2.Controls.Add(control);
            }
        }

        private void Instance_RedisInstanceConnected(object sender, TEventArgs<RedisInstance> e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => { Instance_RedisInstanceConnected(sender, e); }));
            }
            else
            {
                string txtName = string.Format("{0}:{1}", e.Item.IPEndPoint.Address, e.Item.IPEndPoint.Port);

                TreeNode node = treeView1.Nodes.Add(txtName, txtName, "redis");
                node.Tag = e.Item;
                foreach(RedisInstanceDatabase db in e.Item.Databases)
                {
                    string key = string.Format("db{0}", db.Index);
                    string Text = string.Format("db{0}[{1}]", db.Index, db.Keys);
                    TreeNode dbNode = node.Nodes.Add(key, Text, db.Keys>0? "database":"database-off","database-selected");
                    dbNode.Tag = db;
                }

                node.Expand();
            }
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    if (_instance == null)
        //    {
        //        _instance = new RedisInstance();
        //        _instance.StatusChanged += _instance_StatusChanged;
        //    }
        //    else if(_instance.Status == RedisInstanceStatus.Connected)
        //    {
        //        return;
        //    }
        //    IPAddress ip = IPAddress.Any;
        //    IPAddress.TryParse("127.0.0.1", out ip);

        //    _instance.Connect(new IPEndPoint(ip, 6379), "123456");
        //}

        private void _instance_StatusChanged(object sender, TargetChangedEventArgs<RedisInstanceStatus> e)
        {
            Console.WriteLine("Status changed:{0}", e.NewTarget);
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    _instance.Disconnect();
        //}

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmConnect connect = new frmConnect();
            connect.ShowDialog(this);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            frmConnect connect = new frmConnect();
            connect.ShowDialog(this);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //RMStudio.Instance.Publish(TREEVIEW_FOCUS_CHANGE, e.Node.Tag);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RMStudio.Instance.Unsubscribe(TREEVIEW_FOCUS_CHANGE, new Action<string, TargetChangedEventArgs<object>>(TreeView_Focus_Changed));
        }

        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            bool isCancelled = true;
            RMStudio.Instance.Publish(TREEVIEW_FOCUS_CHANGE, e.Node.Tag, out isCancelled);
            e.Cancel = isCancelled;
        }
    }
}
