namespace System.Windows.Forms.Attributes
{
    /// <summary>
    /// Provides configuration options for the GenericDictionaryEditor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    public class GenericDictionaryEditorAttribute : Attribute
    {
        /// <summary>
        /// Constructor for GenericDictionaryEditorAttribute
        /// </summary>
        /// <param name="keyDisplayName">Display Name for Key</param>
        /// <param name="valueDisplayName">Display Name for Value</param>
        public GenericDictionaryEditorAttribute(string keyDisplayName, string valueDisplayName)
        {
            this.KeyDisplayName = keyDisplayName;
            this.ValueDisplayName = valueDisplayName;
        }

        /// <summary>
        /// Default Constructor for GenericDictionaryEditorAttribute
        /// </summary>
        public GenericDictionaryEditorAttribute() : this("Key", "Value") { }

        /// <summary>
        /// Specifies what type to use as a <see cref="DefaultProvider{T}"/> for the keys in the dictionary.
        /// </summary>
        public Type KeyDefaultProviderType { get; set; }

        /// <summary>
        /// Specifies what type to use as a <see cref="System.ComponentModel.TypeConverter">converter</see> for the keys in the dictionary.
        /// </summary>
        public Type KeyConverterType { get; set; }

        /// <summary>
        /// Specifies what type to use as an <see cref="System.Drawing.Design.UITypeEditor">editor</see> to change the key of a dictionary entry.
        /// </summary>
        public Type KeyEditorType { get; set; }

        /// <summary>
        /// Specifies what type to use as an <see cref="AttributeProvider"/> for the keys in the dictionary.
        /// </summary>
        public Type KeyAttributeProviderType { get; set; }

        /// <summary>
        /// Specifies what type to use as a <see cref="DefaultProvider{T}"/> for the values in the dictionary.
        /// </summary>
        public Type ValueDefaultProviderType { get; set; }

        /// <summary>
        /// Specifies what type to use as a <see cref="System.ComponentModel.TypeConverter">converter</see> for the values in the dictionary.
        /// </summary>
        public Type ValueConverterType { get; set; }

        /// <summary>
        /// Specifies what type to use as an <see cref="System.Drawing.Design.UITypeEditor">editor</see> to change the value of a dictionary entry.
        /// </summary>
        public Type ValueEditorType { get; set; }

        /// <summary>
        /// Specifies what type to use as <see cref="AttributeProvider"/> for the values in the dictionary.
        /// </summary>
        public Type ValueAttributeProviderType { get; set; }

        /// <summary>
        /// The title for the editor window.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The display name for key property (defaults to "Key")
        /// </summary>
        public string KeyDisplayName { get; set; }

        /// <summary>
        /// The display name for value property (defaults to "Value")
        /// </summary>
        public string ValueDisplayName { get; set; }
    }
}
