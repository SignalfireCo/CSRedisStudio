using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Management
{
    public abstract class FocusObserver
    {
        private List<string> _keys = new List<string>();
        public List<string> Keys
        {
            get { return _keys; }
        }

        public FocusObserver(string key)
            : this(new string[] { key})
        {

        }

        public FocusObserver(string[] keys)
        {

        }

        public abstract void OnFocusChanging(string key, TargetChangingEventArgs<object> args);
        public abstract void OnFocusChanged(string key, TargetChangedEventArgs<object> args);
    }
}
