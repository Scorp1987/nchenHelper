using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Telegram.Bot.Enums;

namespace System
{
    public static class SystemExtension
    {
        private static readonly char[] CODE_SPECIAL_CHARACTERS = { '\\', '`' };
        private static readonly char[] LINK_SPECIAL_CHARACTERS = { '\\', ')' };
        private static readonly char[] TEXT_SPECIAL_CHARACTERS = { '`', '>', '#', '+', '-', '=', '{', '}', '.', '!' };
        private static readonly char[] OTHER_SPECIAL_CHARACTERS = { '\\', '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };

        public static string RemoveMentions(this string text, string userName) => text.Replace($"@{userName}", "");

        public static string ToText(this object value, IDictionary<Type, TypeConverter> converters = null)
        {
            if (converters != null && converters.TryGetValue(value.GetType(), out var converter))
                return converter.ConvertToString(value);
            else if (value is DateTime)
                return new ClearDateTimeConverter().ConvertToString(value);
            else if (value.IsFloatNumberType())
                return new FloatNumberConverter().ConvertToString(value);
            else
                return TypeDescriptor.GetConverter(value.GetType()).ConvertToString(value);
        }

        public static string ToMarkDownV2Text(this string input, IEnumerable<char> specialCharacters)
        {
            foreach (var specialChar in specialCharacters)
                input = input.Replace($"{specialChar}", $@"\{specialChar}");
            return input;
        }

        public static string ToInlineCode(this string input) => $"`{input.FixInLineCode()}`";

        public static string FixInLineCode(this string input) => input.ToMarkDownV2Text(CODE_SPECIAL_CHARACTERS);

        public static string FixAllOthers(this string input) => input.ToMarkDownV2Text(OTHER_SPECIAL_CHARACTERS);

        public static string FixText(this string input) => input.ToMarkDownV2Text(TEXT_SPECIAL_CHARACTERS);

        public static string ToFormatText(this string input, TextFormat format)
        {
            var prefix = ""; var suffix = "";
            if (format.HasFlag(TextFormat.Spoiler)) { prefix = $"||{prefix}"; suffix = $"{suffix}||"; }
            if (format.HasFlag(TextFormat.Underline)) { prefix = $"__{prefix}"; suffix = $"{suffix}__"; }
            if (format.HasFlag(TextFormat.Strikethrough)) { prefix = $"~{prefix}"; suffix = $"{suffix}~"; }
            if (format.HasFlag(TextFormat.Italic)) { prefix = $"_{prefix}"; suffix = $"{suffix}_"; }
            if (format.HasFlag(TextFormat.Bold)) { prefix = $"*{prefix}"; suffix = $"{suffix}*"; }
            return prefix + input.ToMarkDownV2Text(OTHER_SPECIAL_CHARACTERS) + suffix;
        }

        public static string ToInlineURL(this string input, string link)
            => $"[{input}]({link.ToMarkDownV2Text(LINK_SPECIAL_CHARACTERS)})";

        private static int GetMaxLength(this DataColumn column, IDictionary<Type, TypeConverter> converters = null, bool includeHeader = true)
        {
            var maxLength = (from DataRow row in column.Table.AsEnumerable()
                             select row[column].ToText(converters).Length).Max();

            if (includeHeader)
                return (maxLength > column.ColumnName.Length) ? maxLength : column.ColumnName.Length;
            else
                return maxLength;
        }

        private static Dictionary<DataColumn, int> GetColumnWidths(this DataTable table, IDictionary<Type, TypeConverter> converters = null, bool includeHeader = true, IEnumerable<string> columnNames = null)
        {
            var toReturn = new Dictionary<DataColumn, int>();
            foreach (DataColumn column in table.Columns)
            {
                if (columnNames?.Contains(column.ColumnName) == false) continue;
                var maxLength = column.GetMaxLength(converters, includeHeader);
                toReturn.Add(column, maxLength);
            }
            return toReturn;
        }

        private static string GetHeaderText(this DataTable table, out Dictionary<DataColumn, int> columnWidths, IDictionary<Type, TypeConverter> converters = null, IEnumerable<string> columnNames = null)
        {
            var toReturn = "|";
            var rowStr = "|";
            columnWidths = new Dictionary<DataColumn, int>(table.Columns.Count);
            foreach (DataColumn column in table.Columns)
            {
                if (columnNames?.Contains(column.ColumnName) == false) continue;
                int maxLength = column.GetMaxLength(converters);
                columnWidths.Add(column, maxLength);
                toReturn += column.ColumnName.PadRight(maxLength).ToMarkDownV2Text(CODE_SPECIAL_CHARACTERS) + "|";
                rowStr += new string('-', maxLength) + "|";
            }
            return $"{toReturn}\n{rowStr}";
        }

        private static string ToMarkDownV2Text(this DataTable table, bool includeHeader = true, IDictionary<Type, TypeConverter> converters = null, IEnumerable<string> columnNames = null)
        {
            if (table.Columns.Count < 1) return string.Empty;

            var toReturn = "";
            Dictionary<DataColumn, int> columnWidths;
            if (includeHeader)
                toReturn = table.GetHeaderText(out columnWidths, converters, columnNames);
            else
                columnWidths = table.GetColumnWidths(converters, false, columnNames);

            // Add Row Data
            foreach (DataRow row in table.Rows)
            {
                var rowStr = "|";
                foreach (DataColumn column in table.Columns)
                {
                    if (columnNames?.Contains(column.ColumnName) == false) continue;
                    var value = row[column];
                    if (value.IsNumberType())
                        rowStr += value.ToText(converters).PadLeft(columnWidths[column]).ToMarkDownV2Text(CODE_SPECIAL_CHARACTERS) + "|";
                    else
                        rowStr += value.ToText(converters).PadRight(columnWidths[column]).ToMarkDownV2Text(CODE_SPECIAL_CHARACTERS) + "|";
                }
                toReturn += "\n" + rowStr;
            }

            if (includeHeader)
                return $"```\n{toReturn}\n```";
            else
                return $"```{toReturn}\n```";
        }

        private static string Append(this string source) => string.IsNullOrEmpty(source) ? source : $"{source}\n";

        public static string AppendText(this string source, string text)
            => source.Append() + text.ToMarkDownV2Text(TEXT_SPECIAL_CHARACTERS);

        public static string AppendDataTable(this string source, DataTable table, bool includeHeader = true, IDictionary<Type, TypeConverter> converters = null, IEnumerable<string> columnNames = null)
            => source.Append() + table.ToMarkDownV2Text(includeHeader, converters, columnNames);
    }
}
