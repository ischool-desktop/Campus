using System;
using System.Collections.Generic;
using System.Text;

namespace Campus.Windows
{
    public enum ValueStatus { Clean, Dirty }

    public class ChangeEventArgs : EventArgs
    {
        public ChangeEventArgs(ValueStatus status)
        {
            Status = status;
        }

        public ValueStatus Status { get; private set; }
    }
}
