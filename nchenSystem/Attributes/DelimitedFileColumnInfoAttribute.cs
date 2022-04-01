namespace System.Attributes
{
    public class DelimitedFileColumnInfoAttribute : NamedAttribute
    {
        public DelimitedFileColumnInfoAttribute() { }

        public DelimitedFileColumnInfoAttribute(string name) : base(name) { }

        public DelimitedFileColumnInfoAttribute(int index) => this.Index = index;

        public int? Index { get; }
    }
}
