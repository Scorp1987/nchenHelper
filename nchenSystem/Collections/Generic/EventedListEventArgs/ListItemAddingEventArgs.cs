namespace System.Collections.Generic
{
    public class ListItemAddingEventArgs<T> : IndexValueCancelEventArgs<T>
    {
        public ListItemAddingEventArgs(int index, T value) : base(index, value) { }
    }
}
