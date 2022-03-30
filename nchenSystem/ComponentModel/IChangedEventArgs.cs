using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.ComponentModel
{
    public interface IChangedEventArgs<T>
    {
        T PreviousValue { get; }

        T CurrentValue { get; }
    }
}
