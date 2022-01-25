using System.Globalization;

namespace System.ComponentModel
{
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
