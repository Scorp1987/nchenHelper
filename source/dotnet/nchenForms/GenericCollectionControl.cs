using System.Windows.Forms.Converters;
using System.Windows.Forms.Providers;

namespace System.Windows.Forms
{
    public class GenericCollectionControl<TObject> : CollectionControl<GenericPropertyGrid<TObject>, TObject>
    {
        public GenericCollectionControl(DefaultProvider<TObject> defaultProvider) : base(defaultProvider) { }
        public GenericCollectionControl() : base() { }
    }

    public class GenericCollectionControl<TObject, TType> : TypeCollectionControl<TypeObjectControl<TObject, TType>, TObject, TType>
    {
        public GenericCollectionControl(
            DefaultProvider<TObject> defaultObjectProvider,
            DefaultProvider<TType> defaultTypeProvider,
            GenericConverter<TObject, TType> converter) : base(defaultObjectProvider, defaultTypeProvider, converter) { }
    }
}
