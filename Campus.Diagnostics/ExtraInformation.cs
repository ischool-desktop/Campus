using System;
using System.Collections.Generic;
using System.Text;

namespace Campus.Diagnostics
{
    public class ExtraInformation
    {
        public ExtraInformation(string name, string data)
        {
            Name = name;
            Data = data;
        }

        public string Name { get; private set; }

        public string Data { get; private set; }
    }
}
