using System.ComponentModel;

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
    }
}
