using System.Globalization;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides a type converter to convert <see cref="System.DateTime"/> objects to and from various other representations.
    /// When convert to <see cref="string"/>, return <see langword="yyyy-MM-dd"/> format string if there is no time component.
    /// <see langword="yyyy-MM-dd HH:mm:ss"/> format string if there is no millisecond
    /// otherwise <see langword="yyyy-MM-dd HH:mm:ss.fff"/> format string.
    /// </summary>
    public class ISODateTimeConverter : DateTimeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null) return "";
                var valueDT = (DateTime)value;
                if (valueDT.TimeOfDay.Ticks == 0)
                    return $"{valueDT:yyyy-MM-dd}";
                else if (valueDT.Millisecond == 0)
                    return $"{valueDT:yyyy-MM-dd HH:mm:ss}";
                else
                    return $"{valueDT:yyyy-MM-dd HH:mm:ss.fff}";
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
