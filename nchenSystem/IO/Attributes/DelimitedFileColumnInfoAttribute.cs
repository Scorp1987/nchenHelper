namespace System.IO.Attributes
{
    public class DelimitedFileColumnInfoAttribute : Attribute
    {
        public DelimitedFileColumnInfoAttribute() { }

        public DelimitedFileColumnInfoAttribute(string name) => this.Name = name;

        public DelimitedFileColumnInfoAttribute(int index) => this.Index = index;

        public string Name { get; }

        public int? Index { get; }
    }
}
