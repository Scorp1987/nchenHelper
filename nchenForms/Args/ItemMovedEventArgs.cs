namespace System.Windows.Forms.Args
{
    public class ItemMovedEventArgs<TObject> : EventArgs
    {
        public ItemMovedEventArgs(TObject @object, int originalPosition, int newPosition)
        {
            this.Object = @object;
            this.OrginalPosition = originalPosition;
            this.NewPosition = newPosition;
        }

        public TObject Object { get; }

        public int OrginalPosition { get; }

        public int NewPosition { get; }
    }
}
