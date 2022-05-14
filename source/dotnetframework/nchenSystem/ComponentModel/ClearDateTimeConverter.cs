using System.Globalization;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides a type converter to convert <see cref="System.DateTime"/> objects to and from various other representations.
    /// When convert to <see cref="string"/>, return <see langword="dd-MMM-yyyy"/> format string if there is no time component.
    /// <see langword="dd-MM-yyyy HH:mm:ss"/> format string if there is no millisecond
    /// otherwise <see langword="dd-MM-yyyy HH:mm:ss.fff"/> format string.
    /// </summary>
    public class ClearDateTimeConverter : DateTimeConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is DateTime dt)
            {
                if (dt.TimeOfDay.Ticks == 0)
                    return dt.ToString("dd-MMM-yyyy");
                else if (dt.Millisecond == 0)
                    return dt.ToString("dd-MMM-yyyy HH:mm:ss");
                else
                    return dt.ToString("dd-MMM-yyyy HH:mm:ss.fff");
            }
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
