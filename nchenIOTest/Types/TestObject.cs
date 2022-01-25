using System.ComponentModel;
using System.IO.Attributes;
using System.IO.Converters;

namespace System.IO.Types
{
    class TestObject
    {
        public static bool operator ==(TestObject a, TestObject b)
        {
            return a.Text == b.Text
                && a.Date == b.Date
                && a.DateTime == b.DateTime
                && a.NullableDateTime == b.NullableDateTime
                && a.Number == b.Number
                && a.NullableNumber == b.NullableNumber
                && a.Object?.Name == b.Object?.Name
                && a.Object?.Value == b.Object?.Value;
        }
        public static bool operator !=(TestObject a, TestObject b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(TestObject)) return false;
            var tobj = (TestObject)obj;
            return tobj == this;
        }

        [WithName("TextName")]
        [WithoutName]
        public string Text { get; set; }

        [WithoutName]
        [DelimitedFileColumnName("DateName")]
        public DateTime Date { get; set; }

        [WithoutName("DateTimeName")]
        public DateTime DateTime { get; set; }

        public DateTime? NullableDateTime { get; set; }

        public int Number { get; set; }

        [WithoutName()]
        public double? NullableNumber { get; set; }

        [WithName("ObjectName")]
        [TypeConverter(typeof(TestSubObjectConverter))]
        public TestSubObject Object { get; set; }
    }
}
