using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisManagementStudio
{
    public class TEventArgs<T> : EventArgs
    {
        private T _item;
        public T Item
        {
            get { return _item; }
        }

        public TEventArgs(T item)
        {
            this._item = item;
        }
    }

    public class TCancelEventArgs<T> : TEventArgs<T>
    {
        private bool _cancelled;
        public bool Cancelled
        {
            get { return _cancelled; }
            set { _cancelled = value; }
        }

        public TCancelEventArgs(T item)
            : base(item)
        {

        }
    }
}
