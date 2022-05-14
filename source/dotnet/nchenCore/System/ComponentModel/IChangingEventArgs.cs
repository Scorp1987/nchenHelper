namespace System.ComponentModel
{
    public interface IChangingEventArgs<T>
    {
        T OriginalValue { get; }

        T NewValue { get; }
    }
}
