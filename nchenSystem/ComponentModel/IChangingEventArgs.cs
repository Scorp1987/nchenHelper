using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public interface IChangingEventArgs<T>
    {
        T OriginalValue { get; }

        T NewValue { get; }
    }
}
