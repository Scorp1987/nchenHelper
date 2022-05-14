namespace System.Windows.Forms.Attributes
{
    /// <summary>
    /// Provides configuration options for the GenericCollectionEditor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class GenericCollectionEditorAttribute : Attribute
    {
        /// <summary>
        /// Specifies what type to use as a <see cref="DefaultProvider{T}"/> for object collection.
        /// </summary>
        public Type DefaultObjectProviderType { get; set; }

        public Type DefaultTypeProviderType { get; set; }

        public Type ConverterType { get; set; }
    }
}
