namespace System.Windows.Forms.Args
{
    public interface IChangedArgs<TValue>
    {
        TValue OldValue { get; }

        TValue NewValue { get; }
    }
}
