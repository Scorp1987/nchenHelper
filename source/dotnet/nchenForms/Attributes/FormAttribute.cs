namespace System.Windows.Forms.Attributes
{
    /// <summary>
    /// Provides configuration options for the Form.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class FormAttribute : Attribute
    {
        public string Title { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool MinimizeBox { get; set; }

        public bool MaximizeBox { get; set; }
    }
}
