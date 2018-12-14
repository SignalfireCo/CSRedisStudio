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
using CSRedis;
using WeifenLuo.WinFormsUI.Docking;

namespace RedisManagementStudio
{
    public partial class MainForm : Form
    {
        private DeserializeDockContent _deserializeDockContent;
        frmExplorer frmExplorer ;

        private Dictionary<string, DockContent> dockContents = new Dictionary<string, DockContent>();

        MainFormDocumentsFocusAbserver _documentsFocusAbserver;

        public MainForm()
        {
            InitializeComponent();

            //RMStudio.Instance.Subscribe(
            //    FocusKeys.STUDIO_FOCUS
            //    , new Action<string, TargetChangingEventArgs<object>>(Studio_Focus_Changing)
            //    ,  new Action<string, TargetChangedEventArgs<object>>(Studio_Focus_Changed)
            // );

            
            this.dockPanel1.Theme = this.vS2015LightTheme1;

            _documentsFocusAbserver = new MainFormDocumentsFocusAbserver(
                new string[]{
                FocusKeys.STUDIO_FOCUS
                , FocusKeys.REDIS_MONITOR
                }, dockPanel1);
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            Console.WriteLine(persistString);

            if(persistString == typeof(frmExplorer).ToString())
            {
                return frmExplorer = new frmExplorer();
            }

            return null;
        }

        private void Studio_Focus_Changing(string key, TargetChangingEventArgs<object> args)
        {
            //if (args.Original!=null && MessageBox.Show("是否取消？", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //{
            //    args.Cancel = true;
            //}
        }

        private void Studio_Focus_Changed(string key, TargetChangedEventArgs<object> args)
        {
            if (args.NewTarget == null) return;

            Console.WriteLine(args.NewTarget.ToString());
            if(args.NewTarget is RedisInstance)
            {
                frmRedis frmRedis = new frmRedis(args.NewTarget as RedisInstance);

                frmRedis.Text = "资源管理器";
                frmRedis.Show(this.dockPanel1);
                //frmRedis.DockTo(this.dockPanel1, DockStyle.Fill);
                //frmRedis.Text = "资源管理器";
            }

            if (args.NewTarget is RedisInstanceDatabase)
            {
                //control = new uctDatabase((RedisInstanceDatabase)args.NewTarget);
            }

        }


        private void _instance_StatusChanged(object sender, TargetChangedEventArgs<RedisInstanceStatus> e)
        {
            Console.WriteLine("Status changed:{0}", e.NewTarget);
        }


        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            frmConnect connect = new frmConnect();
            connect.ShowDialog(this);
        }

        RedisClient client = null;
        private void MainForm_Load(object sender, EventArgs e)
        {
            if (System.IO.File.Exists("layout.sf"))
            {
                _deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
                dockPanel1.LoadFromXml("layout.sf", _deserializeDockContent);
            }

            if (frmExplorer == null)
            {
                frmExplorer = new frmExplorer();
                frmExplorer.Show(this.dockPanel1);
                frmExplorer.DockTo(this.dockPanel1, DockStyle.Left);
            }

            frmConnect connect = new frmConnect();
            connect.ShowDialog(this);

            //client = new RedisClient("127.0.0.1", 6379);
            //client.Auth("123456");
            //client.MonitorReceived += Client_MonitorReceived;
            //Task task = new Task(new Action(client.Monitor));
            //Func<string> func = new Func<string>(client.Monitor);
            //func.BeginInvoke(null, null);
            //client.Monitor();

        }

        private void Client_MonitorReceived(object sender, RedisMonitorEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //RMStudio.Instance.Publish(TREEVIEW_FOCUS_CHANGE, e.Node.Tag);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //RMStudio.Instance.Unsubscribe(TREEVIEW_FOCUS_CHANGE, new Action<string, TargetChangedEventArgs<object>>(TreeView_Focus_Changed));
            try
            {
                string rtn = client.Echo("ABC");
                Console.WriteLine(rtn);
            }catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            dockPanel1.SaveAsXml("layout.sf");
        }

        //private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        //{
        //    bool isCancelled = true;
        //    RMStudio.Instance.Publish(TREEVIEW_FOCUS_CHANGE, e.Node.Tag, out isCancelled);
        //    e.Cancel = isCancelled;
        //}
    }
}
