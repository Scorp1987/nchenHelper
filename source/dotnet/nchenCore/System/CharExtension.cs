using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class CharExtension
    {
        public static bool? ToBoolean(this char ch) => $"{ch}".ToBoolean();
    }
}
