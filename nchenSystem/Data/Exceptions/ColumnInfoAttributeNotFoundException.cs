namespace System.Data.Exceptions
{
    class ColumnInfoAttributeNotFoundException<TObject> : Exception
    {
        public ColumnInfoAttributeNotFoundException(string propertyName) : base($"{typeof(TObject).Name} is not found for {propertyName}") { }
    }
}
