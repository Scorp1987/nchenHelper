using System.ComponentModel;

namespace System.Collections.Generic
{
    public class IndexValueCancelEventArgs<T> : CancelEventArgs, IIndex, IValue<T>
    {
        public IndexValueCancelEventArgs(int index, T value)
        {
            this.Index = index;
            this.Value = value;
        }

        public int Index { get; }

        public T Value { get; }
    }
}
