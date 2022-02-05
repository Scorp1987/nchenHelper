using System.ComponentModel;
using System.Data.Attributes;
using System.Data.Exceptions;
using System.Linq;

namespace System.Reflection
{
    public static class PropertyInfoExtension
    {
        /// <summary>
        /// Get Type Converter of the Property of an object.
        /// If TypeConverterAttribute is in the model then will use the convertertype declare in the model,
        /// otherwise will use the default type converter
        /// </summary>
        /// <param name="propertyInfo">Property of an object</param>
        /// <returns>TypeConverter object</returns>
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
        /// Get the name of the column for property with <typeparamref name="TAttribute"/>
        /// </summary>
        /// <typeparam name="TAttribute">
        /// <see cref="DataColumnInfoAttribute"/> type for the property
        /// </typeparam>
        /// <param name="property">Property of an object</param>
        /// <returns>
        /// <see cref="MemberInfo.Name"/> if <see cref="DataColumnInfoAttribute.Name"/> is <see langword="null"/>,
        /// otherwise <see cref="DataColumnInfoAttribute.Name"/>
        /// </returns>
        public static string GetDataTableColumnName<TAttribute>(this PropertyInfo property)
            where TAttribute : DataColumnInfoAttribute
        {
            var attribute = GetAttribute<TAttribute>(property);

            return attribute?.Name ?? property.Name;
        }

        public static string GetDataTableDbDataType<TAttribute>(this PropertyInfo property)
            where TAttribute : DataColumnInfoAttribute
        {
            var attribute = GetAttribute<TAttribute>(property);
            return GetDbDataType(property, attribute);
        }

        private static TAttribute GetAttribute<TAttribute>(this PropertyInfo property)
            where TAttribute : DataColumnInfoAttribute
            => property.GetCustomAttributes<TAttribute>().FirstOrDefault();

        private static string GetDbDataType(this PropertyInfo property, DataColumnInfoAttribute attribute)
        {
            if (string.IsNullOrEmpty(attribute.DbDataType)) throw new EmptyDataTypeException(property.Name);
            return attribute.GetDbDataType();
        }
    }
}
