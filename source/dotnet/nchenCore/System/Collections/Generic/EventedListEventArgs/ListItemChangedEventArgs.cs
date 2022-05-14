using System.ComponentModel;

namespace System.Collections.Generic
{
    public class ListItemChangedEventArgs<T> : ChangedEventArgs<T>, IIndex
    {
        public ListItemChangedEventArgs(int index, T previousValue, T currentValue) : base(previousValue, currentValue) => Index = index;

        public int Index { get; }
    }
}
