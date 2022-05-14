namespace System.ComponentModel
{
    public interface IChangedEventArgs<T>
    {
        T PreviousValue { get; }

        T CurrentValue { get; }
    }
}
