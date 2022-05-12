using AdaptiveCards.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdaptiveCards.Types
{
    public class TableColumnWidth
    {
        public static implicit operator TableColumnWidth(string str)
        {
            if (str.Equals("Auto", StringComparison.InvariantCultureIgnoreCase))
                return new TableColumnWidth { Type = TableColumnWidthType.Auto };
            else
            {
                var match = Regex.Match(str, @"^\s*Max\s*=\s*(?<Value>[0-9]*)\s*$");
                if (match.Success)
                    return new TableColumnWidth
                    {
                        Type = TableColumnWidthType.Max,
                        Value = int.Parse(match.Groups["Value"].Value),
                    };
                else if (int.TryParse(str, out var widthInt))
                    return new TableColumnWidth
                    {
                        Type = TableColumnWidthType.Absolute,
                        Value = widthInt,
                    };
                else
                    throw new NotSupportedException($"Can't convert '{str}' to {nameof(TableColumnWidth)}");
            }
        }

        public static implicit operator TableColumnWidth(float value) =>
            new TableColumnWidth
            {
                Type = TableColumnWidthType.Absolute,
                Value = (int)value,
            };

        public TableColumnWidthType Type { get; set; } = TableColumnWidthType.Auto;

        public int Value { get; set; }

        public object GetWidthObject()
        {
            switch (Type)
            {
                case TableColumnWidthType.Auto:
                    return "Auto";
                case TableColumnWidthType.Absolute:
                    return Value;
                case TableColumnWidthType.Max:
                    return $"Max={Value}";
                default:
                    throw new NotImplementedException();
            }
        }

        public override string ToString() => GetWidthObject().ToString();
    }
}
