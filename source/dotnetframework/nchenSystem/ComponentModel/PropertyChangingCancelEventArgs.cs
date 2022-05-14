namespace System.ComponentModel
{
    public class PropertyChangingCancelEventArgs : PropertyChangingEventArgs, ICancelEventArgs
    {
        public PropertyChangingCancelEventArgs(string propertyName) : base(propertyName) { }

        public bool Cancel { get; set; }
    }

    public class PropertyChangingCancelEventArgs<T> : PropertyChangingCancelEventArgs, IChangingEventArgs<T>
    {
        public PropertyChangingCancelEventArgs(string propertyName, T originalValue, T newValue) : base(propertyName)
        {
            OriginalValue = originalValue;
            NewValue = newValue;
        }

        public T OriginalValue { get; }

        public T NewValue { get; }
    }
}
