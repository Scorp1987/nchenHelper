namespace System.ComponentModel
{
    public class ChangedEventArgs<T> : EventArgs, IChangedEventArgs<T>
    {
        public ChangedEventArgs(T previousValue, T currentValue)
        {
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }

        public T PreviousValue { get; }

        public T CurrentValue { get; }
    }
}
