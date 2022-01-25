using System.Globalization;

namespace System.ComponentModel
{
    public class FloatNumberConverter : DecimalConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => (destinationType == typeof(string)) ? ((decimal)value).ToString("0.000") : base.ConvertTo(context, culture, value, destinationType);
    }
}
