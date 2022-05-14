namespace System.ComponentModel
{
    public class PropertyChangedEventArgs<T> : PropertyChangedEventArgs, IChangedEventArgs<T>
    {
        public PropertyChangedEventArgs(string propertyName, T previousValue, T currentValue) : base(propertyName)
        {
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }

        public T PreviousValue { get; }

        public T CurrentValue { get; }
    }
}
