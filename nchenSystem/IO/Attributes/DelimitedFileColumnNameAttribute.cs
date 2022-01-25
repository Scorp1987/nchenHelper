namespace System.IO.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DelimitedFileColumnNameAttribute : Attribute
    {
        public DelimitedFileColumnNameAttribute() { }

        public DelimitedFileColumnNameAttribute(string name) => this.Name = name;

        public string Name { get; }
    }
}
