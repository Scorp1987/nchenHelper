namespace System.Windows.Forms.Args
{
    public class ItemMovedEventArgs<TObject> : EventArgs
    {
        public ItemMovedEventArgs(TObject @object, int previousIndex, int currentIndex)
        {
            this.Object = @object;
            this.PreviousIndex = previousIndex;
            this.CurrentIndex = currentIndex;
        }

        public TObject Object { get; }

        public int PreviousIndex { get; }

        public int CurrentIndex { get; }
    }
}
