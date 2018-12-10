using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisManagementStudio
{
    public class FocusChangedEventArgs : EventArgs
    {
        private object _original;
        public object Original
        {
            get { return _original; }
        }

        private object _newFocus;
        public object NewFocus
        {
            get { return _newFocus; }
        }

        public FocusChangedEventArgs(object orginal, object newFocus)
        {
            this._newFocus = newFocus;
            this._original = orginal;
        }
    }
}
