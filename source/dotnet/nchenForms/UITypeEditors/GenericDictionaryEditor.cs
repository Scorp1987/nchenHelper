using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Attributes;
using System.Windows.Forms.UITypeEditors.GenericDictionaryEditors;

namespace System.Windows.Forms.UITypeEditors
{
    public class GenericDictionaryEditor<Tkey, TValue> : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (!(value is IDictionary<Tkey, TValue> dictionary)) throw new ArgumentException("Invalid Type", nameof(value));
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes[typeof(GenericDictionaryEditorAttribute)] is GenericDictionaryEditorAttribute dicAttrib)
            {
                if (dicAttrib.KeyDefaultProviderType == null)
                    dicAttrib.KeyDefaultProviderType = typeof(DefaultProvider<Tkey>);
                if (dicAttrib.ValueDefaultProviderType == null)
                    dicAttrib.ValueDefaultProviderType = typeof(DefaultProvider<TValue>);
            }
            else
                dicAttrib = new GenericDictionaryEditorAttribute
                {
                    KeyDefaultProviderType = typeof(DefaultProvider<Tkey>),
                    ValueDefaultProviderType = typeof(DefaultProvider<TValue>)
                };

            if (context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes[typeof(FormAttribute)] is FormAttribute frmAttrib) { }
            else frmAttrib = null;

            using var control = new GenericDictionaryControl<Tkey, TValue>(dicAttrib);
            control.Value = dictionary;
            if (frmAttrib == null) control.ShowDialog();
            else control.ShowDialog(frmAttrib);
            return base.EditValue(context, provider, value);
        }
    }
}
