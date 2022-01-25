namespace System.Collections.Generic
{
    public class ListItemRemovedEventArgs<T> : EventArgs
    {
        public ListItemRemovedEventArgs(int index, T value)
        {
            this.Index = index;
            this.Value = value;
        }

        public int Index { get; }

        public T Value { get; }
    }
}
