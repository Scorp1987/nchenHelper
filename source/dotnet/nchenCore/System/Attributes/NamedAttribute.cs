namespace System.Attributes
{
    public class NamedAttribute : Attribute
    {
        public NamedAttribute() { }

        public NamedAttribute(string name) => this.Name = name;

        /// <summary>
        /// Column Name
        /// </summary>
        public string Name { get; set; }
    }
}
