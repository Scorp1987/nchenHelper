using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public class ListRangeItemChangingEventArgs : CancelEventArgs, IIndex, ICount
    {
        public ListRangeItemChangingEventArgs(int index, int count)
        {
            this.Index = index;
            this.Count = count;
        }

        public int Index { get; }

        public int Count { get; }
    }
}
