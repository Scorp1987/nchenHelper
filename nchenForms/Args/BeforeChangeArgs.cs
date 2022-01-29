namespace System.Windows.Forms.Args
{
    public class BeforeChangeArgs<TValue> : ChangedArgs<TValue>, ICancelArgs
    {
        public BeforeChangeArgs(TValue oldValue, TValue newValue) : base(oldValue, newValue) { }

        public bool Cancel { get; set; }
    }
}
