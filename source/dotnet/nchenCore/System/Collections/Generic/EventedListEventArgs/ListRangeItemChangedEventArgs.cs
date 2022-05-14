namespace System.Collections.Generic
{
    public class ListRangeItemChangedEventArgs : EventArgs, IIndex, ICount
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
