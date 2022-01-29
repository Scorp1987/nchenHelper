
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

        public MappingType MappingType { get; set; } = Data.MappingType.Element;

        public bool AllowDBNull { get; set; } = true;

        public bool Unique { get; set; } = false;

        public string DbDataType { get; set; }

        public object DefaultValue { get; set; }

        public virtual string GetDbDataType() => DbDataType;
    }
}
