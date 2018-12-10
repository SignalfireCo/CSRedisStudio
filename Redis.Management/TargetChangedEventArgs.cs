﻿using System;
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

        public TargetChangedEventArgs(T original, T newTarget)
        {
            this._original = original;
            this._newTarget = newTarget;
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

        public TargetChangingEventArgs(T original, T newTarget)
        {
            this._newTarget = newTarget;
            this._original = original;
        }
    }
}
