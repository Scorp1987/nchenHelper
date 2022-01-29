using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.Args;
using System.Windows.Forms.Attributes;

namespace System.Windows.Forms.UITypeEditors.GenericDictionaryEditors
{
    internal class EditableKeyValuePair<TKey, TValue> : CustomTypeDescriptor
    {
        private TKey _key;
        public TKey Key
        {
            get => _key;
            set
            {
                //if (_key.Equals(value)) return;
                var arg = new BeforeChangeArgs<TKey>(_key, value);
                KeyBeforeChanged?.Invoke(this, arg);
                if (arg.Cancel) return;
                _key = value;
                KeyChanged?.Invoke(this, arg);
            }
        }

        private TValue _value;
        public TValue Value
        {
            get => _value;
            set
            {
                //if (_value.Equals(value)) return;
                var orgValue = _value;
                _value = value;
                ValueChanged?.Invoke(this, new ChangedArgs<TValue>(orgValue, value));
            }
        }

        public event EventHandler<BeforeChangeArgs<TKey>> KeyBeforeChanged;
        public event EventHandler<ChangedArgs<TKey>> KeyChanged;
        public event EventHandler<ChangedArgs<TValue>> ValueChanged;

        public EditableKeyValuePair(TKey key, TValue value, GenericDictionaryEditorAttribute editorAttribute)
        {
            Key = key;
            Value = value;

            EditorAttribute = editorAttribute ?? throw new ArgumentNullException(nameof(editorAttribute));
        }

        public GenericDictionaryEditorAttribute EditorAttribute { get; set; }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();

        public override PropertyDescriptorCollection GetProperties()
        {
            var properties = new List<PropertyDescriptor>();

            var keyDescriptor = new KeyValueDescriptor(TypeDescriptor.CreateProperty(GetType(), "Key", typeof(TKey)), EditorAttribute.KeyConverterType, EditorAttribute.KeyEditorType, EditorAttribute.KeyAttributeProviderType, EditorAttribute.KeyDisplayName);
            properties.Add(keyDescriptor);

            var valueDescriptor = new KeyValueDescriptor(TypeDescriptor.CreateProperty(GetType(), "Value", typeof(TValue)), EditorAttribute.ValueConverterType, EditorAttribute.ValueEditorType, EditorAttribute.ValueAttributeProviderType, EditorAttribute.ValueDisplayName);
            properties.Add(valueDescriptor);

            return new PropertyDescriptorCollection(properties.ToArray());
        }

        public override object GetPropertyOwner(PropertyDescriptor pd) => this;

        public override string ToString() => $"[{this.Key},{this.Value}]";
    }
}
