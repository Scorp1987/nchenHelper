namespace System.Data.Exceptions
{
    class EmptyDataTypeException : Exception
    {
        public EmptyDataTypeException(string propertyName) : base($"DataType is null or empty for {propertyName} property.") { }
    }
}
