namespace System.IO.Attributes
{
    class WithoutNameAttribute : DelimitedFileColumnInfoAttribute
    {
        public WithoutNameAttribute() { }

        public WithoutNameAttribute(string name) : base(name) { }
    }
}
