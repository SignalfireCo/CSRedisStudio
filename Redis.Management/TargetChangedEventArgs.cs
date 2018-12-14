using System;
using System.Collections.Generic;
using System.Text;

namespace Redis.Management
{
    public class TargetChangedEventArgs<T> : EventArgs
    {
        private T _original;
        public T Original
        {
            get { return _original; }
        }

        private T _newTarget;
        public T NewTarget
        {
            get { return _newTarget; }
        }

        private object _context;
        public object Context
        {
            get { return _context; }
        }


        public TargetChangedEventArgs(T original, T newTarget)
            :this(original, newTarget, null)
        {

        }

        public TargetChangedEventArgs(T original, T newTarget, object context)
        {
            this._original = original;
            this._newTarget = newTarget;
            _context = context;
        }
    }

    public class TargetChangingEventArgs<T> : EventArgs
    {
        public bool Cancel { get; set; }

        private T _original;
        public T Original
        {
            get { return _original; }
        }
        private T _newTarget;
        public T NewTarget
        {
            get { return _newTarget; }
        }

        private object _context;
        public object Context
        {
            get { return _context; }
        }

        public TargetChangingEventArgs(T original, T newTarget)
            : this(original, newTarget, null)
        {

        }

        public TargetChangingEventArgs(T original, T newTarget, object context)
        {
            this._newTarget = newTarget;
            this._original = original;
            _context = context;
        }
    }
}
