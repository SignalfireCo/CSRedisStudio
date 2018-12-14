using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redis.Management;

namespace RedisManagementStudio
{
    public class RMStudio
    {
        public static RMStudio Instance = new RMStudio();

        public event EventHandler<TEventArgs<Redis.Management.RedisInstance>> RedisInstanceConnected;

        public void OnRedisInstanceConnected(TEventArgs<Redis.Management.RedisInstance> args)
        {
            RedisInstanceConnected?.Invoke(this, args);
        }

        private Dictionary<string, Action<string, TargetChangedEventArgs<object>>> _changedActions = new Dictionary<string, Action<string, TargetChangedEventArgs<object>>>();
        private Dictionary<string, Action<string, TargetChangingEventArgs<object>>> _changingActions = new Dictionary<string, Action<string, TargetChangingEventArgs<object>>>();
        private Dictionary<string, object> _values = new Dictionary<string, object>();

        public RMStudio()
        {

        }

        public void Subscribe(string key, Action<string, TargetChangedEventArgs<object>> changed)
        {
            Subscribe(key, null, changed);
        }

        public void Subscribe(string key, Action<string, TargetChangingEventArgs<object>> changing, Action<string, TargetChangedEventArgs<object>> changed)
        {
            if (changed != null)
            {
                if (!_changedActions.ContainsKey(key))
                {
                    _changedActions[key] = changed;
                }
                else
                {
                    _changedActions[key] = (Action<string, TargetChangedEventArgs<object>>)Delegate.Combine(_changedActions[key], changed);
                }
            }
            if (changing != null)
            {
                if (!_changingActions.ContainsKey(key))
                {
                    _changingActions[key] = changing;
                }
                else
                {
                    _changingActions[key] = (Action<string, TargetChangingEventArgs<object>>)Delegate.Combine(_changingActions[key], changing);

                }
            }
        }

        public void Unsubscribe(string key, Action<string, TargetChangedEventArgs<object>> changed)
        {
            Unsubscribe(key, null, changed);
        }
        public void Unsubscribe(string key, Action<string, TargetChangingEventArgs<object>> changing, Action<string, TargetChangedEventArgs<object>> changed)
        {
            if (changed != null)
            {
                if (_changedActions.ContainsKey(key)) 
                    _changedActions[key] = (Action<string, TargetChangedEventArgs<object>>)Delegate.Remove(_changedActions[key], changed);
            }
            
            if (changing != null)
            {
                if (_changingActions.ContainsKey(key)) 
                    _changingActions[key] = (Action<string, TargetChangingEventArgs<object>>)Delegate.Remove(_changingActions[key], changing);
            }

        }

        public void Publish(string key, object value, out bool isCancelled, object context =null)
        {
            isCancelled = false;

            if (_changingActions.ContainsKey(key) && _changingActions[key]!=null)
            {
                TargetChangingEventArgs<object> args = new TargetChangingEventArgs<object>(_values.ContainsKey(key)?_values[key]:null, value, context);
                _changingActions[key](key, args);
                if (args.Cancel)
                {
                    isCancelled = true;
                    return;
                }
            }
            object original = _values.ContainsKey(key)? _values[key]:null;
            _values[key] = value;
            if(_changedActions.ContainsKey(key) && _changedActions[key] != null)
            {
                _changedActions[key](key, new TargetChangedEventArgs<object>(original, value, context));
            }

        }
    }
}
