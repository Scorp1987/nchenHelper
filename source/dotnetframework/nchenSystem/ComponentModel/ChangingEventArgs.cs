namespace System.ComponentModel
{
    public class ChangingEventArgs<T> : CancelEventArgs, ICancelEventArgs, IChangingEventArgs<T>
    {
        public ChangingEventArgs(T originalValue, T newValue)
        {
            OriginalValue = originalValue;
            NewValue = newValue;
        }

        public T OriginalValue { get; }

        public T NewValue { get; }
    }
}
