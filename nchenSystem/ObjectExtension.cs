using System.Collections.Generic;
using System.IO.Types;
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
    }
}
