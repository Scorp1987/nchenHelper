using System.Globalization;

namespace System.ComponentModel
{
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
