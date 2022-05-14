using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms.Attributes;

namespace System.Windows.Forms.UITypeEditors.GenericDictionaryEditors
{
    internal class EditableKeyValuePair<TKey, TValue> : CustomTypeDescriptor
    {
        private TKey _key;
        private TValue _value;

        public TKey Key
        {
            get => _key;
            set => SetPropertyValue(ref _key, value, KeyChanging, KeyChanged);
        }

        public TValue Value
        {
            get => _value;
            set => SetPropertyValue(ref _value, value, ValueChanged);
        }

        public event EventHandler<ChangingEventArgs<TKey>> KeyChanging;
        public event EventHandler<ChangedEventArgs<TKey>> KeyChanged;
        public event EventHandler<ChangedEventArgs<TValue>> ValueChanged;

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

        private bool OnPropertyChanging<T>(T originalValue, T newValue,
            EventHandler<ChangingEventArgs<T>> handler)
        {
            if (handler == null) return true;
            var args = new ChangingEventArgs<T>(originalValue, newValue);
            handler(this, args);
            return !args.Cancel;
        }

        private void OnPropertyChanged<T>(T previousValue, T currentValue,
            EventHandler<ChangedEventArgs<T>> handler)
            => handler?.Invoke(this, new ChangedEventArgs<T>(previousValue, currentValue));

        private bool SetPropertyValue<T>(ref T originalValue, T newValue,
            EventHandler<ChangingEventArgs<T>> changingHandler,
            EventHandler<ChangedEventArgs<T>> changedHandler)
        {
            if (originalValue?.Equals(newValue) == true) return false;
            if (!OnPropertyChanging(originalValue, newValue, changingHandler)) return false;
            var previousValue = originalValue;
            originalValue = newValue;
            OnPropertyChanged(previousValue, newValue, changedHandler);
            return true;
        }

        private bool SetPropertyValue<T>(ref T originalValue, T newValue,
            EventHandler<ChangedEventArgs<T>> changedHandler)
        {
            if (originalValue?.Equals(newValue) == true) return false;
            var previousValue = originalValue;
            originalValue = newValue;
            OnPropertyChanged(previousValue, newValue, changedHandler);
            return true;
        }
    }
}
