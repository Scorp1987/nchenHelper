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

        public bool GetAllowDBNull(TypeConverter converter) => converter is NullableConverter || (this.Attribute?.AllowDBNull ?? true);

        public bool GetUnique() => this.Attribute?.Unique ?? false;

        public int GetMaxLength() => (Attribute is StringDataColumnInfoAttribute sa) ? sa.MaxLength : -1;

        public string GetSqlDataType(Type dataType, int? maxLength)
        {
            if (!string.IsNullOrEmpty(Attribute?.DbDataType)) return Attribute?.DbDataType;
            else if (dataType == typeof(string)) return $"VARCHAR({(maxLength.HasValue ? maxLength.Value.ToString() : "MAX")})";
            else if (dataType == typeof(bool)) return "BIT";
            else if (dataType == typeof(byte)) return "TINYINT";
            else if (dataType == typeof(Int16)) return "SMALLINT";
            else if (dataType == typeof(Int32)) return "INT";
            else if (dataType == typeof(Int64)) return "BIGINT";
            else if (dataType == typeof(float)) return "FLOAT";
            else if (dataType == typeof(double)) return "DOUBLE";
            else return "";
        }

        public string GetSqlDataType(Type dataType)
        {
            int? maxLength;
            if (Attribute is StringDataColumnInfoAttribute sa) maxLength = sa.MaxLength;
            else maxLength = null;
            return GetSqlDataType(dataType, maxLength);
        }

        public object GetDefaultValue(Type dataType, bool allowDbNull)
        {
            if (dataType == typeof(string) && !allowDbNull) return "";
            else return this.Attribute?.DefaultValue ?? default;
        }
    }
}
