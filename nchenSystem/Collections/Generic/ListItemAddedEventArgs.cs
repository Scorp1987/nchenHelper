namespace System.Collections.Generic
{
    public class ListItemAddedEventArgs<T> : EventArgs
    {
        public ListItemAddedEventArgs(int index, T value)
        {
            this.Index = index;
            this.Value = value;
        }

        public int Index { get; }

        public T Value { get; }
    }
}
