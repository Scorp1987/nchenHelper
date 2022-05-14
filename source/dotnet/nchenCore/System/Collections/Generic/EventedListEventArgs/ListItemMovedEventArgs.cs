namespace System.Collections.Generic
{
    public class ListItemMovedEventArgs<T> : EventArgs, IValue<T>
    {
        public ListItemMovedEventArgs(T value, int previousIndex, int currentIndex)
        {
            this.Value = value;
            this.PreviousIndex = previousIndex;
            this.CurrentIndex = currentIndex;
        }

        public T Value { get; }

        public int PreviousIndex { get; }

        public int CurrentIndex { get; }
    }
}
