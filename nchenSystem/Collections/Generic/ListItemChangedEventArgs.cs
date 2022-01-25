namespace System.Collections.Generic
{
    public class ListItemChangedEventArgs<T> : EventArgs
    {
        public ListItemChangedEventArgs(int index, T orgValue, T newValue)
        {
            this.Index = index;
            this.OrgValue = orgValue;
            this.NewValue = newValue;
        }

        public int Index { get; }

        public T OrgValue { get; }

        public T NewValue { get; }
    }
}
