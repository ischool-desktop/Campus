using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Campus.ePaper
{
    public static class tool
    {
        public static int PullIntTry(string a)
        {
            int k = 1;
            int.TryParse(a, out k);
            return k;
        }

    }
}
