namespace System.IO.Attributes
{
    class WithoutNameAttribute : DelimitedFileColumnNameAttribute
    {
        public WithoutNameAttribute() { }

        public WithoutNameAttribute(string name) : base(name) { }
    }
}
