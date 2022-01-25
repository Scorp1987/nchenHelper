namespace System.Data.Attributes
{
    /// <summary>
    /// Column Information to be used in property of TObject
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DataColumnInfoAttribute : Attribute
    {
        /// <summary>
        /// Column Name
        /// </summary>
        public string Name { get; set; }

        public string Expression { get; set; }

        public MappingType? MappingType { get; set; }

        public bool AllowDBNull { get; set; }
    }
}
