using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Redis.Management;

namespace RedisManagementStudio
{
    public partial class frmConnect : Form
    {
        private CancellationTokenSource _cancellationTokenSource;

        public frmConnect()
        {
            InitializeComponent();
        }

        private void bttnCancel_Click(object sender, EventArgs e)
        {
            if(_cancellationTokenSource!=null && _cancellationTokenSource.Token.CanBeCanceled)
            {
                _cancellationTokenSource.Cancel();

                txtHost.Enabled = true;
                txtPassword.Enabled = true;
                txtPort.Enabled = true;
                bttnConnect.Enabled = true;
                return;
            }
            this.Close();
        }

        private void bttnConnect_Click(object sender, EventArgs e)
        {

            IPAddress ip = IPAddress.Any;
            if(txtHost.Text.Trim().Equals(string.Empty) || !IPAddress.TryParse(txtHost.Text.Trim(), out ip))
            {
                MessageBox.Show("请输入正确的服务器地址！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int port = 0;
            if(txtPort.Text.Trim().Equals(string.Empty) || !int.TryParse(txtPort.Text, out port))
            {
                MessageBox.Show("请输入正确的端口值！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            IPEndPoint ep = new IPEndPoint(ip, port);


            txtHost.Enabled = false;
            txtPassword.Enabled = false;
            txtPort.Enabled = false;
            bttnConnect.Enabled = false;

            _cancellationTokenSource = new CancellationTokenSource();
            Task<RedisInstance> task = new Task<RedisInstance>(()=> Connecting(ep, txtPassword.Text), _cancellationTokenSource.Token);
            
            task.Start();            

            task.ContinueWith(t => {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() => {
                        if (t.Result != null && _cancellationTokenSource!=null && !_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            RMStudio.Instance.OnRedisInstanceConnected(new TEventArgs<RedisInstance>(t.Result));
                            this.Close();
                        }
                        else
                        {
                            txtHost.Enabled = true;
                            txtPassword.Enabled = true;
                            txtPort.Enabled = true;
                            bttnConnect.Enabled = true;
                            _cancellationTokenSource = null;
                        }
                    }));
                }
            });


        }

        private RedisInstance Connecting(IPEndPoint endpoint, string password)
        {
            RedisInstance instance = new RedisInstance();
            try
            {
                bool isConnected = instance.Connect(endpoint, password);
                if (!isConnected)
                {
                    MessageBox.Show("无法连接到服务器，请确认服务器正常运行。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }catch(Exception ex)
            {
                Action action = new Action(() => {
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new MethodInvoker(() => {
                            MessageBox.Show(string.Format("连接服务器过程错误。错误信息：{0}",ex.Message), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }));
                    }
                });

                action();
            }

            return instance.Status == RedisInstanceStatus.Connected? instance:null;
        }
    }
}
