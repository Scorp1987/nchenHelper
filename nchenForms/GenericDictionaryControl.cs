using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms.Args;
using System.Windows.Forms.Attributes;
using System.Windows.Forms.UITypeEditors.GenericDictionaryEditors;

namespace System.Windows.Forms
{
    public partial class GenericDictionaryControl<Tkey, TValue> : UserControl, IObjectControl<IDictionary<Tkey, TValue>>
    {
        public GenericDictionaryControl(GenericDictionaryEditorAttribute editorAttribute)
        {
            InitializeComponent();
            this.EditorAttribute = editorAttribute;
        }
        public GenericDictionaryControl(GenericDictionaryEditorAttribute editorAttribute, IDictionary<Tkey, TValue> value)
            : this(editorAttribute) => this.Value = value;
        public GenericDictionaryControl(IDictionary<Tkey, TValue> value) : this(new GenericDictionaryEditorAttribute
        {
            KeyDefaultProviderType = typeof(DefaultProvider<Tkey>),
            ValueDefaultProviderType = typeof(DefaultProvider<TValue>)
        }, value)
        { }
        public GenericDictionaryControl() : this(new Dictionary<Tkey, TValue>()) { }


        [Category("Appearance")]
        [Description("Title of the Control")]
        [DefaultValue("Title")]
        public string Title { get => TitleToolStripLabel.Text; set => TitleToolStripLabel.Text = value; }


        [Category("Behavior")]
        [Description("Controls whether the control can be changed or not")]
        [DefaultValue(false)]
        public bool ReadOnly { get => !this.Enabled; set => this.Enabled = !value; }


        private IDictionary<Tkey, TValue> _value;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public IDictionary<Tkey, TValue> Value
        {
            get => _value;
            set
            {
                if (value == null) value = new Dictionary<Tkey, TValue>();
                _value = value;

                if (this.BindingSource != null)
                {
                    var orgList = (IList<EditableKeyValuePair<Tkey, TValue>>)BindingSource.DataSource;
                    foreach (var item in orgList)
                        UnSubscribeItemEvents(item);
                    this.BindingSource.Dispose();
                }

                List<EditableKeyValuePair<Tkey, TValue>> newList;
                if (value == null)
                    newList = new List<EditableKeyValuePair<Tkey, TValue>>();
                else
                {
                    newList = new List<EditableKeyValuePair<Tkey, TValue>>(value.Count);
                    foreach (var item in value)
                    {
                        var kvp = new EditableKeyValuePair<Tkey, TValue>(item.Key, item.Value, this.EditorAttribute);
                        SubscribeItemEvents(kvp);
                        newList.Add(kvp);
                    }
                }
                this.BindingSource = new BindingSource { DataSource = newList };
                this.CollectionListBox.DataSource = this.BindingSource;
            }
        }


        protected BindingSource BindingSource { get; set; }
        protected GenericDictionaryEditorAttribute EditorAttribute { get; }


        public void AddToolStripItem(ToolStripItem item) => toolStrip1.Items.Add(item);
        public void RemoveToolStripItem(ToolStripItem item) => toolStrip1.Items.Remove(item);
        public void AddItem(Tkey key, TValue value)
        {
            if (IsKeyExisted(key)) return;
            try { Value.Add(key, value); }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return;
            }
            var item = new EditableKeyValuePair<Tkey, TValue>(key, value, this.EditorAttribute);
            SubscribeItemEvents(item);
            this.BindingSource.Add(item);
        }
        public void AddItem(Tkey key)
        {
            var value = GetDefaultValue();
            AddItem(key, value);
        }
        public void RemoveItem(Tkey key)
        {
            var kvp = (from item in (IList<EditableKeyValuePair<Tkey, TValue>>)this.BindingSource.DataSource
                       where item.Key.Equals(key)
                       select item).First();
            RemoveItem(kvp);
        }
        private void RemoveItem(EditableKeyValuePair<Tkey, TValue> item)
        {
            try { this.Value.Remove(item.Key); }
            catch (Exception ex)
            {
                ShowError(ex.Message);
                return;
            }

            UnSubscribeItemEvents(item);
            this.BindingSource.Remove(item);
        }


        private Tkey GetDefaultKey()
        {
            if (Activator.CreateInstance(this.EditorAttribute.KeyDefaultProviderType) is DefaultProvider<Tkey> keyDefaultProvider)
                return keyDefaultProvider.GetDefault(DefaultUsage.Key);
            else
                return default;
        }
        private TValue GetDefaultValue()
        {
            if (Activator.CreateInstance(this.EditorAttribute.ValueDefaultProviderType) is DefaultProvider<TValue> valueDefaultProvider)
                return valueDefaultProvider.GetDefault(DefaultUsage.Value);
            else
                return default;
        }
        private bool ShowError(string message)
        {
            MessageBox.Show(message, "Invalid Key", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
        }
        private bool IsKeyExisted(Tkey key)
        {
            if (Value.ContainsKey(key))
                return !ShowError($"{key} key is already existed.");
            return false;
        }

        private void SubscribeItemEvents(EditableKeyValuePair<Tkey, TValue> item)
        {
            item.KeyChanging += EditableKeyValuePair_KeyBeforeChanged;
            item.KeyChanged += EditableKeyValuePair_KeyChanged;
            item.ValueChanged += EditableKeyValuePair_ValueChanged;
        }
        private void UnSubscribeItemEvents(EditableKeyValuePair<Tkey, TValue> item)
        {
            item.KeyChanging -= EditableKeyValuePair_KeyBeforeChanged;
            item.KeyChanged -= EditableKeyValuePair_KeyChanged;
            item.ValueChanged -= EditableKeyValuePair_ValueChanged;
        }


        private void CollectionListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            var item = this.CollectionListBox.SelectedItem;
            this.propertyGrid1.SelectedObject = item;
        }

        private void AddToolStripButton_Click(object sender, EventArgs e)
        {
            var key = GetDefaultKey();
            var value = GetDefaultValue();
            AddItem(key, value);
        }
        private void DeleteToolStripButton_Click(object sender, EventArgs e)
        {
            if (!(this.CollectionListBox.SelectedItem is EditableKeyValuePair<Tkey, TValue> item)) return;
            RemoveItem(item);
        }

        private void EditableKeyValuePair_KeyBeforeChanged(object sender, ChangingEventArgs<Tkey> e)
        {
            e.Cancel = IsKeyExisted(e.NewValue);
        }
        private void EditableKeyValuePair_KeyChanged(object sender, ChangedEventArgs<Tkey> e)
        {
            var value = this.Value[e.PreviousValue];
            this.Value.Remove(e.CurrentValue);
            this.Value.Add(e.CurrentValue, value);
            this.BindingSource.ResetBindings(false);
        }
        private void EditableKeyValuePair_ValueChanged(object sender, ChangedEventArgs<TValue> e)
        {
            var item = (EditableKeyValuePair<Tkey, TValue>)sender;
            this.Value[item.Key] = e.CurrentValue;
            this.BindingSource.ResetBindings(false);
        }
    }
}
