namespace System.Windows.Forms.Attributes
{
    /// <summary>
    /// Provides configuration options for the FilePathEditor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class FilePathEditorAttribute : Attribute
    {
        public string Title { get; set; }
        public string InitialDirectory { get; set; }
        public string FileName { get; set; }
        public string DefaultExt { get; set; }
        public string Filter { get; set; }
        public bool MultiSelect { get; set; }
    }
}
