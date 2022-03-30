using System.ComponentModel;

namespace System.Collections.Generic
{
    public class ListItemMovingEventArgs<T>: CancelEventArgs, IValue<T>
    {
        public ListItemMovingEventArgs(T value, int originalIndex, int newIndex)
        {
            Value = value;
            OriginalIndex = originalIndex;
            NewIndex = newIndex;
        }

        public T Value { get; }

        public int OriginalIndex { get; }

        public int NewIndex { get; }
    }
}
