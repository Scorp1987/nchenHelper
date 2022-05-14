namespace System.Collections.Generic
{
    public class ListItemRemovedEventArgs<T> : IndexValueEventArgs<T>
    {
        public ListItemRemovedEventArgs(int index, T value) : base(index, value) { }
    }
}
