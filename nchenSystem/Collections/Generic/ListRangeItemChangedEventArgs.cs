namespace System.Collections.Generic
{
    public class ListRangeItemChangedEventArgs : EventArgs
    {
        public ListRangeItemChangedEventArgs(int index, int count)
        {
            this.Index = index;
            this.Count = count;
        }

        public int Index { get; }

        public int Count { get; }
    }
}
