namespace System.Collections.Generic
{
    public class IndexValueEventArgs<T> : EventArgs, IIndex, IValue<T>
    {
        public IndexValueEventArgs(int index, T value)
        {
            this.Index = index;
            this.Value = value;
        }

        public int Index { get; }

        public T Value { get; }
    }
}
