using System.Windows.Forms.Converters;
using System.Windows.Forms.Providers;

namespace System.Windows.Forms
{
    public class TypeCollectionControl<TControl, TObject, TType> : CollectionControl<TControl, TObject>
        where TControl : TypeObjectControl<TObject, TType>, new()
    {
        public TypeCollectionControl(
            DefaultProvider<TObject> defaultObjectProvider,
            DefaultProvider<TType> defaultTypeProvider,
            GenericConverter<TObject, TType> converter
            ) : base(defaultObjectProvider)
        {
            this.ObjectControl.DefaultTypeProvider = defaultTypeProvider;
            this.ObjectControl.Converter = converter;
            this.ObjectControl.TypeChanged += ObjectControl_TypeChanged;
        }
        public TypeCollectionControl(DefaultProvider<TObject> defaultObjectProvider) : base(defaultObjectProvider)
        {
            this.ObjectControl.TypeChanged += ObjectControl_TypeChanged;
        }

        private void ObjectControl_TypeChanged(object sender, TObject e)
        {
            if (!(base.CollectionListBox.SelectedItem is TObject para)) return;
            var index = base.BindingSource.IndexOf(para);
            base.BindingSource[index] = e;
        }
    }
}
