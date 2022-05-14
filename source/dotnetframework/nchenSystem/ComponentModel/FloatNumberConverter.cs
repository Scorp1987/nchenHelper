using System.Globalization;

namespace System.ComponentModel
{
    /// <summary>
    /// Provides a type converter to convert System.Decimal objects to and from various other representations.
    /// When convert to <see cref="string"/>, return <see langword="0.000"/> format string.
    /// </summary>
    public class FloatNumberConverter : DecimalConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            => (destinationType == typeof(string)) ? ((decimal)value).ToString("0.000") : base.ConvertTo(context, culture, value, destinationType);
    }
}
