namespace System.IO.Types
{
    class TestSubObject
    {
        public string Name { get; set; }

        public int Value { get; set; }

        public override string ToString() => Name;
    }
}
