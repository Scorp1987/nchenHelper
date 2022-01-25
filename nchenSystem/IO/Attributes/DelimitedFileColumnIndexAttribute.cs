namespace System.IO.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DelimitedFileColumnIndexAttribute : Attribute
    {
        public int Index { get; set; }

        public DelimitedFileColumnIndexAttribute(int index) => this.Index = index;
    }
}
