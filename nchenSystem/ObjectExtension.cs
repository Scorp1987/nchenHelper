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
            else return value.ToString();
        }

        public static string ToSqlValueString(this object value, bool allowDbNull)
        {
            if (value is DateTime dt) return dt.ToSqlValueString();
            else if (value is string str)
                return (string.IsNullOrEmpty(str) && allowDbNull) ? "NULL" : $"'{str}'";
            else return value.ToString();
        }
    }
}
