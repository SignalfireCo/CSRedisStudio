using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.Management;
using WeifenLuo.WinFormsUI.Docking;

namespace RedisManagementStudio
{
    public class MainFormDocumentsFocusAbserver:FocusObserver
    {
        public DockPanel DockPanel { get; set; }

        private frmRedisMonitors _monitorForm;

        public MainFormDocumentsFocusAbserver(string key, DockPanel dockPanel) : base(key)
        {
            DockPanel = dockPanel;


            RMStudio.Instance.Subscribe(
                key
                , new Action<string, TargetChangingEventArgs<object>>(OnFocusChanging)
                , new Action<string, TargetChangedEventArgs<object>>(OnFocusChanged)
             );

        }

        public MainFormDocumentsFocusAbserver(string[] keys, DockPanel dockPanel) : base(keys)
        {
            DockPanel = dockPanel;

            foreach(string key in keys)
            {
                RMStudio.Instance.Subscribe(
                   key
                   , new Action<string, TargetChangingEventArgs<object>>(OnFocusChanging)
                   , new Action<string, TargetChangedEventArgs<object>>(OnFocusChanged)
                );

            }
        }


        public override void OnFocusChanged(string key, TargetChangedEventArgs<object> args)
        {
            switch (key)
            {
                case FocusKeys.STUDIO_FOCUS:
                    if (args.NewTarget == null) return;


                    IDockContent current = null;
                    foreach (IDockContent content in this.DockPanel.Documents)
                    {
                        if (content.DockHandler.Form.Tag == args.NewTarget)
                        {
                            current = content;
                            current.DockHandler.Activate();
                            break;
                        }
                    }

                    if (current != null) return;



                    Console.WriteLine(args.NewTarget.ToString());
                    if (args.NewTarget is RedisInstance)
                    {
                        frmRedis frmRedis = new frmRedis(args.NewTarget as RedisInstance);
                        frmRedis.Show(DockPanel);
                        frmRedis.Tag = args.NewTarget;
                    }

                    if (args.NewTarget is RedisInstanceDatabase)
                    {
                        frmDatabase database = new frmDatabase(args.NewTarget as RedisInstanceDatabase);
                        database.Show(this.DockPanel);
                        database.Tag = args.NewTarget;

                    }

                    if (args.NewTarget is string)
                    {
                        switch (args.NewTarget.ToString().ToLower())
                        {
                            case "databases":
                                frmDatabases frmDatabases = new frmDatabases(args.Context as RedisInstance);
                                frmDatabases.Show(this.DockPanel);
                                frmDatabases.Tag = args.NewTarget;
                                break;

                            default:
                                break;
                        }
                    }

                    break;

                case FocusKeys.REDIS_MONITOR:
                    if (_monitorForm == null || _monitorForm.IsDisposed)
                    {
                        frmRedisMonitors frmRedisMonitors = new frmRedisMonitors((RedisInstance)args.NewTarget);
                        frmRedisMonitors.Show(DockPanel);
                        frmRedisMonitors.DockTo(DockPanel, System.Windows.Forms.DockStyle.Bottom);
                        _monitorForm = frmRedisMonitors;
                    }
                    else
                    {
                        frmRedisMonitors frmRedisMonitors = new frmRedisMonitors((RedisInstance)args.NewTarget);
                        frmRedisMonitors.Show(((frmRedisMonitors)_monitorForm).Pane, _monitorForm);
                        //frmRedisMonitors.DockTo(monitorForm.DockPanel, System.Windows.Forms.DockStyle.Bottom);
                    }

                    break;
            }


        }
        public override void OnFocusChanging(string key, TargetChangingEventArgs<object> args)
        {
            
        }
    }
}
