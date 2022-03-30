namespace System.Collections.Generic
{
    public class ListItemAddedEventArgs<T> : IndexValueEventArgs<T>
    {
        public ListItemAddedEventArgs(int index, T value) : base(index, value) { }
    }
}
