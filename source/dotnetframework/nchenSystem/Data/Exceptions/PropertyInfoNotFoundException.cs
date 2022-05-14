namespace System.Data.Exceptions
{
    class PropertyInfoNotFoundException : Exception
    {
        public PropertyInfoNotFoundException(string propertyName) : base($"Property is not found for {propertyName}") { }
    }
}
