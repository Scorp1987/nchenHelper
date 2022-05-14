using System.ComponentModel;

namespace System.Collections.Generic
{
    public class ListItemChangingEventArgs<T> : ChangingEventArgs<T>, IIndex
    {
        public ListItemChangingEventArgs(int index, T originalValue, T newValue) : base(originalValue, newValue) => Index = index;

        public int Index { get; }
    }
}
