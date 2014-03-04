using System;
using System.Collections.Generic;
using System.Text;

namespace Campus.Diagnostics
{
    public interface IExtraProcesser
    {
        ExtraInformation[] Process(object instance);
    }
}
