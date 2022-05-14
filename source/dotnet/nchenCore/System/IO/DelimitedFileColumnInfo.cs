using System.Reflection;

namespace System.IO
{
    public class DelimitedFileColumnInfo
    {
        public string Name { get; set; }
        public int? Index { get; set; }
        public PropertyInfo Property { get; set; }

        public override string ToString() => Name;
    }
}
