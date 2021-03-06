using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Reflection;

namespace System
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert from shortform Path to full file/folder Path
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToPath(this string str)
        {
            if (str == null) return null;
            var regex = new Regex("([%][^%]+[%])");
            var path = regex.Replace(str, m =>
            {
                var value = m.Value[1..^1];
                var specialFolder = (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), value, true);
                return Environment.GetFolderPath(specialFolder);
            });
            if (!path.Contains(":")) path = Path.Combine(Directory.GetCurrentDirectory(), path);
            return path;
        }

        /// <summary>
        /// Convert from full file/folder path to shortform path
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToShortFormPath(this string str)
        {
            if (str == null) return null;
            var toReturn = str;
            toReturn = toReturn.Replace(AppDomain.CurrentDomain.BaseDirectory, "");
            foreach (Environment.SpecialFolder sf in Enum.GetValues(typeof(Environment.SpecialFolder)))
            {
                var spfp = Environment.GetFolderPath(sf);
                if (!string.IsNullOrEmpty(spfp))
                    toReturn = toReturn.Replace(Environment.GetFolderPath(sf), $"%{sf}%");
            }
            return toReturn;
        }

        public static bool? ToBoolean(this string str)
        {
            if (bool.TryParse(str, out var b)) return b;
            else if (str == "0") return false;
            else if (str == "1") return true;
            else return null;
        }

        public static byte? ToByte(this string str)
        {
            if (byte.TryParse(str, out var b)) return b;
            else return null;
        }

        public static int? ToInt(this string str)
        {
            if (int.TryParse(str, out var i)) return i;
            else return null;
        }

        public static float ToFloat(this string str)
        {
            if (float.TryParse(str, out var f)) return f;
            else return float.NaN;
        }

        public static double ToDouble(this string str)
        {
            if (double.TryParse(str, out var d)) return d;
            else return double.NaN;
        }

        public static decimal? ToDecimal(this string str)
        {
            if (decimal.TryParse(str, out var d)) return d;
            else return null;
        }

        public static DateTime? ToDateTime(this string str)
        {
            if (DateTime.TryParse(str, out var dt)) return dt;
            else return null;
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of
        /// one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <typeparam name="TEnum">An enumeration type</typeparam>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <returns>An <typeparamref name="TEnum"/> whose value is represented by <paramref name="value"/>.</returns>
        public static TEnum ToEnum<TEnum>(this string value)
            where TEnum : struct
        {
            if (int.TryParse(value, out _)) return Enum.Parse<TEnum>(value);

            var enumType = typeof(TEnum);
            foreach(var name in Enum.GetNames(enumType))
            {
                var enumMemberAttribute = enumType.GetField(name).GetCustomAttribute<EnumMemberAttribute>(true);
                if ((enumMemberAttribute == null && name == value) || (enumMemberAttribute != null && enumMemberAttribute.Value == value))
                    return Enum.Parse<TEnum>(name);
            }
            throw new NotSupportedException($"Can't convert '{value}' to {enumType.Name}.");
        }
    }
}
