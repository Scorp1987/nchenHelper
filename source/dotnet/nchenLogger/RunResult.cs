using System;
using System.Collections.Generic;
using System.Text;

namespace System.IO
{
    public enum RunResult
    {
        Default = 0,
        Successful,
        Timeout,
        Cancel,
        Error
    }
}
