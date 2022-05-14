using System.Attributes;
using System.ComponentModel;
using System.Linq;

namespace System.Reflection
{
    public static class PropertyInfoExtension
    {
        /// <summary>
        /// Return Get <see cref="TypeConverter"/> for the current member.
        /// </summary>
        /// <param name="propertyInfo">Property of an object</param>
        /// <returns>
        /// <para>
        /// <see cref="TypeConverterAttribute.ConverterTypeName"/> converter
        /// if current property have <see cref="TypeConverterAttribute"/> attribute.
        /// </para>
        /// <para>
        /// default <see cref="TypeConverter"/> converter
        /// if current property doesn't have <see cref="TypeConverterAttribute"/> attribute.
        /// </para>
        /// </returns>
        public static TypeConverter GetTypeConverter(this PropertyInfo propertyInfo)
        {
            var attr = propertyInfo.GetCustomAttribute<TypeConverterAttribute>();
            if (attr != null)
            {
                var converterTypeName = attr.ConverterTypeName;
                var converterType = Type.GetType(converterTypeName);
                return (TypeConverter)Activator.CreateInstance(converterType);
            }
            else
                return TypeDescriptor.GetConverter(propertyInfo.PropertyType);
        }


        /// <summary>
        /// Gets the name of the current member
        /// </summary>
        /// <param name="property"></param>
        /// <returns>A <see langword="string"/> containing the name of the current member.</returns>
        public static string GetName(this PropertyInfo property) => property.Name;
        /// <summary>
        /// Gets the name of the current member with the specified <paramref name="attribute"/>
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        /// <returns>
        /// <see cref="MemberInfo.Name"/> if <see cref="NamedAttribute.Name"/> is <see langword="null"/>,
        /// otherwise <see cref="NamedAttribute.Name"/>
        /// </returns>
        public static string GetName(this PropertyInfo property, NamedAttribute attribute) => attribute?.Name ?? property.Name;
        /// <summary>
        /// Gets the name for property which have <typeparamref name="TAttribute"/> attribute
        /// </summary>
        /// <typeparam name="TAttribute">
        /// <see cref="NamedAttribute"/> type for the property
        /// </typeparam>
        /// <param name="property">Property of an object</param>
        /// <returns>
        /// <see cref="MemberInfo.Name"/> if <see cref="NamedAttribute.Name"/> is <see langword="null"/>,
        /// otherwise <see cref="NamedAttribute.Name"/>
        /// </returns>
        public static string GetName<TAttribute>(this PropertyInfo property)
            where TAttribute : NamedAttribute
        {
            var attribute = property.GetAttribute<TAttribute>();
            return property.GetName(attribute);
        }


        /// <summary>
        /// Gets the equivalent datatype of the sql database for the current member.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetSqlDataType(this PropertyInfo property)
        {
            var dataType = property.PropertyType;
            if (dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(Nullable<>))
                dataType = Nullable.GetUnderlyingType(dataType);

            if (dataType == typeof(bool)) return "BIT";
            else if (dataType == typeof(byte)) return "TINYINT";
            else if (dataType == typeof(Int16)) return "SMALLINT";
            else if (dataType == typeof(Int32)) return "INT";
            else if (dataType == typeof(Int64)) return "BIGINT";
            else if (dataType == typeof(float)) return "FLOAT";
            else if (dataType == typeof(double)) return "FLOAT";
            else if (dataType == typeof(string)) return $"VARCHAR(MAX)";
            else if (dataType == typeof(DateTime)) return "DATETIME";
            else throw new NotImplementedException($"{dataType.Name} is not implemented.");
        }
        /// <summary>
        /// Gets the equivalent datatype of the sql database for the current member.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public static string GetSqlDataType(this PropertyInfo property, DataColumnInfoAttribute attribute)
        {
            var dbDataType = attribute?.GetDbDataType();
            if (!string.IsNullOrEmpty(dbDataType)) return dbDataType;

            return property.GetSqlDataType();
        }
        /// <summary>
        /// Gets the equivalent datatype of the sql database for the current member.
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="property"></param>
        /// <returns></returns>
        public static string GetSqlDataType<TAttribute>(this PropertyInfo property)
            where TAttribute : DataColumnInfoAttribute
        {
            var attribute = property.GetAttribute<TAttribute>();
            return property.GetSqlDataType(attribute);
        }


        public static TAttribute GetAttribute<TAttribute>(this PropertyInfo property)
            where TAttribute : Attribute
            => property.GetCustomAttributes<TAttribute>().FirstOrDefault();
    }
}
