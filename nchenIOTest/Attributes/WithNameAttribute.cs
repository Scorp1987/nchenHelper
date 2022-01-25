namespace System.IO.Attributes
{
    class WithNameAttribute : DelimitedFileColumnNameAttribute
    {
        public WithNameAttribute(string name) : base(name) { }
    }
}
