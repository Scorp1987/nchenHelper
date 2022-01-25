using System.ComponentModel;
using System.Data.Attributes;
using System.Reflection;

namespace System.Data.Types
{
    class PropertyAttributeInfo
    {
        public PropertyInfo Property { get; set; }

        public DataColumnInfoAttribute Attribute { get; set; }

        public string GetColumnName() => this.Attribute?.Name ?? this.Property.Name;

        public string GetExpression() => this.Attribute?.Expression;

        public MappingType GetMappingType() => this.Attribute?.MappingType ?? MappingType.Element;

        public TypeConverter GetConverter() => TypeDescriptor.GetConverter(this.Property.PropertyType);

        public Type GetDataType(TypeConverter converter) => (converter is NullableConverter nc) ? nc.UnderlyingType : this.Property.PropertyType;

        public bool GetAllowDBNull(TypeConverter converter) => (converter is NullableConverter || this.Attribute?.AllowDBNull == true);
    }
}
