namespace System.Collections.Generic
{
    public class ListItemRemovingEventArgs<T> : IndexValueCancelEventArgs<T>
    {
        public ListItemRemovingEventArgs(int index, T value): base(index, value) { }
    }
}
