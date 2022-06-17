using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class ObjectExtension
    {
        public static bool IsSignedIntegerNumberType(this object value) =>
            value is sbyte ||
            value is short ||
            value is int ||
            value is long;
        public static bool IsUnsignedIntegerNumberType(this object value) =>
            value is byte ||
            value is ushort ||
            value is uint ||
            value is ulong;
        public static bool IsIntegerNumberType(this object value) => value.IsSignedIntegerNumberType() || value.IsUnsignedIntegerNumberType();
        public static bool IsFloatNumberType(this object value) =>
            value is float ||
            value is double ||
            value is decimal;
        public static bool IsNumberType(this object value) => value.IsIntegerNumberType() || value.IsFloatNumberType();

        public static string ToSqlValueString(this object value)
        {
            if (value is DateTime dt) return dt.ToSqlValueString();
            else if (value is string str) return $"'{str}'";
            else if (value is bool b) return b ? "1" : "0";
            else return value.ToString();
        }
        public static string ToSqlValueString(this object value, bool allowDbNull)
        {
            if (value is DateTime dt) return dt.ToSqlValueString();
            else if (value is string str)
                return (string.IsNullOrEmpty(str) && allowDbNull) ? "NULL" : $"'{str}'";
            else if (value is bool b) return b ? "1" : "0";
            else return value.ToString();
        }

        private static object GetDeepPropertyValue(this object instance, string[] propNames)
        {
            var type = instance.GetType();
            foreach(var propName in propNames)
            {
                if (type.IsGenericType)
                {
                    var interfaces = type.GetGenericTypeDefinition().GetInterfaces();
                    if(interfaces.Count(i => i == typeof(IDictionary)) > 0)
                    {
                        var getItem = type.GetMethod("get_Item");
                        instance = getItem.Invoke(instance, new object[] { propName });
                        type = instance.GetType();
                    }
                }
                else
                {
                    PropertyInfo propInfo = type.GetProperty(propName);
                    if (propInfo != null)
                    {
                        instance = propInfo.GetValue(instance, null);
                        type = propInfo.PropertyType;
                    }
                    else throw new ArgumentException($"'{propName}' property is not found in '{type.Name}' type.");
                }
            }
            return instance;
        }
        public static object GetDeepPropertyValue(this object instance, string path)
        {
            var propPaths = path.Split('.');
            return instance.GetDeepPropertyValue(propPaths);
        }
        public static void SetDeepPropertyValue(this object instance, string path, object value)
        {
            var propNames = path.Split('.');
            instance = instance.GetDeepPropertyValue(propNames[0..^1]);
            var type = instance.GetType();
            var propName = propNames[^1];
            if (type.IsGenericType)
            {
                var interfaces = type.GetGenericTypeDefinition().GetInterfaces();
                if (interfaces.Count(i => i == typeof(IDictionary)) > 0)
                {
                    var setItem = type.GetMethod("set_Item");
                    setItem.Invoke(instance, new object[] { propName, value });
                }
                else
                {
                    PropertyInfo propInfo = type.GetProperty(propName);
                    if (propInfo != null)
                        propInfo.SetValue(instance, value);
                    else throw new ArgumentException($"'{propName}' property is not found in '{type.Name}' type.");
                }
            }
        }

        public static void UpdateObject<TObject>(this TObject @object, IEnumerable<DelimitedFileColumnInfo> columnInfos, string[] values)
        {
            if (columnInfos.Count() != values.Count()) throw new ArgumentException("number of columnInfos and values must the be the same");
            foreach (var columnInfo in columnInfos)
            {
                if (columnInfo.Property == null) continue;
                var converter = columnInfo.Property.GetTypeConverter();
                var value = converter.ConvertFrom(values[columnInfo.Index.Value]);
                columnInfo.Property.SetValue(@object, value);
            }
        }

        public static T[][] ToJaggedArray<T>(this T[] source, int maxWidth)
        {
            var height = Math.Ceiling((double)source.Length / (float)maxWidth);
            var output = new List<T[]>();
            for (int row = 0; row < height; row++)
            {
                var start = row * maxWidth;
                var end = (row + 1) * maxWidth;
                if (end > source.Length) end = source.Length;
                output.Add(source[start..end]);
            }
            return output.ToArray();
        }
    }
}
