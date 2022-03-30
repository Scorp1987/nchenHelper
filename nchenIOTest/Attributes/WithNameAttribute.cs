namespace System.IO.Attributes
{
    class WithNameAttribute : DelimitedFileColumnInfoAttribute
    {
        public WithNameAttribute(string name) : base(name) { }
    }
}
