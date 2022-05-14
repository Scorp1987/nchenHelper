namespace System.ComponentModel
{
    public static class PropertyChangedEventArgsExtension
    {
        public static T GetPreviousValue<T>(this PropertyChangedEventArgs e)
        {
            if (e is PropertyChangedEventArgs<T> args) return args.PreviousValue;
            else throw new NotSupportedException(
                $"{nameof(args.PreviousValue)} is only avaliable for {nameof(PropertyChangedEventArgs<T>)}." +
                $"Current type is {e.GetType().Name}");
        }
        public static object GetPreviousValue(this PropertyChangedEventArgs e)
        {
            var type = e.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PropertyChangedEventArgs<>))
                return type.GetProperty("PreviousValue").GetValue(e);
            else throw new NotSupportedException(
                $"PreviousValue is not avaliable for {type.Name}");
        }


        public static T GetCurrentValue<T>(this PropertyChangedEventArgs e)
        {
            if (e is PropertyChangedEventArgs<T> args) return args.CurrentValue;
            else throw new NotSupportedException(
                $"{nameof(args.CurrentValue)} is only avalibale for {nameof(PropertyChangedEventArgs<T>)}." +
                $"Current type is {e.GetType().Name}");
        }
        public static object GetCurrentValue(this PropertyChangedEventArgs e)
        {
            var type = e.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PropertyChangedEventArgs<>))
                return type.GetProperty("CurrentValue").GetValue(e);
            else throw new NotSupportedException(
                $"CurrentValue is not avaliable for {type.Name}");
        }
    }
}
