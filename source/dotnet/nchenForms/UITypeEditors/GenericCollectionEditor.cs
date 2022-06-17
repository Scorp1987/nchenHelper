using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Attributes;
using System.Windows.Forms.Converters;
using System.Windows.Forms.Providers;

namespace System.Windows.Forms.UITypeEditors
{
    public class GenericCollectionEditor<TObject> : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (!(value is ICollection<TObject> collection)) throw new ArgumentException("Invalid Type", nameof(value));
            if (context == null) throw new ArgumentNullException(nameof(context));

            Type defaultProviderType;
            if (context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes[typeof(GenericCollectionEditorAttribute)] is GenericCollectionEditorAttribute colAttrib)
            {
                if (colAttrib.DefaultObjectProviderType == null)
                    defaultProviderType = typeof(DefaultProvider<TObject>);
                else
                    defaultProviderType = colAttrib.DefaultObjectProviderType;
            }
            else
                defaultProviderType = typeof(DefaultProvider<TObject>);

            if (!(Activator.CreateInstance(defaultProviderType) is DefaultProvider<TObject> defaultProvider))
                defaultProvider = new DefaultProvider<TObject>();

            if (context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes[typeof(FormAttribute)] is FormAttribute frmAttrib) { }
            else frmAttrib = null;

            using var control = new GenericCollectionControl<TObject>(defaultProvider);
            control.Value = collection;
            if (frmAttrib == null) control.ShowDialog();
            else control.ShowDialog(frmAttrib);
            return base.EditValue(context, provider, value);
        }
    }

    public class GenericCollectionEditor<TObject, TType> : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.Modal;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (!(value is ICollection<TObject> collection)) throw new ArgumentException("Invalid Type", nameof(value));
            if (context == null) throw new ArgumentNullException(nameof(context));

            Type defaultObjectProviderType;
            Type defaultTypeProviderType;
            Type converterType;
            if (context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes[typeof(GenericCollectionEditorAttribute)] is GenericCollectionEditorAttribute colAttrib)
            {
                defaultObjectProviderType = colAttrib.DefaultObjectProviderType ?? typeof(DefaultProvider<TObject>);
                defaultTypeProviderType = colAttrib.DefaultTypeProviderType ?? typeof(DefaultProvider<TType>);
                converterType = colAttrib.ConverterType ?? typeof(GenericConverter<TObject, TType>);
            }
            else
            {
                defaultObjectProviderType = typeof(DefaultProvider<TObject>);
                defaultTypeProviderType = typeof(DefaultProvider<TType>);
                converterType = typeof(GenericConverter<TObject, TType>);
            }

            if (!(Activator.CreateInstance(defaultObjectProviderType) is DefaultProvider<TObject> defaultObjectProvider))
                defaultObjectProvider = new DefaultProvider<TObject>();
            if (!(Activator.CreateInstance(defaultTypeProviderType) is DefaultProvider<TType> defaultTypeProvider))
                defaultTypeProvider = new DefaultProvider<TType>();
            if (!(Activator.CreateInstance(converterType) is GenericConverter<TObject, TType> converter))
                converter = new GenericConverter<TObject, TType>();


            if (context.PropertyDescriptor != null && context.PropertyDescriptor.Attributes[typeof(FormAttribute)] is FormAttribute frmAttrib) { }
            else frmAttrib = null;

            using (var control = new GenericCollectionControl<TObject, TType>(defaultObjectProvider, defaultTypeProvider, converter))
            {
                control.Value = collection;
                if (frmAttrib == null) control.ShowDialog();
                else control.ShowDialog(frmAttrib);
            }
            return base.EditValue(context, provider, value);
        }
    }
}
