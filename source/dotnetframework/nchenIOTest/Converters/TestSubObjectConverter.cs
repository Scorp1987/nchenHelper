using System.ComponentModel;
using System.Globalization;
using System.IO.Types;

namespace System.IO.Converters
{
    class TestSubObjectConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null) return null;
            else if (value is string str)
            {
                var arr = str.Split('|');
                return new TestSubObject
                {
                    Name = arr[0],
                    Value = int.Parse(arr[1])
                };
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null) return "";
                var obj = (TestSubObject)value;
                return $"{obj.Name}|{obj.Value}";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
