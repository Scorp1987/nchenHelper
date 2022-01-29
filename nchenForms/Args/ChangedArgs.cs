namespace System.Windows.Forms.Args
{
    public class ChangedArgs<TValue> : EventArgs, IChangedArgs<TValue>
    {
        public ChangedArgs(TValue oldValue, TValue newValue)
        {
            this.OldValue = oldValue;
            this.NewValue = newValue;
        }

        public TValue OldValue { get; }

        public TValue NewValue { get; }
    }
}
