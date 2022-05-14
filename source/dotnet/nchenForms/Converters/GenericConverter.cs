namespace System.Windows.Forms.Converters
{
    public class GenericConverter<TFrom, TTo>
    {
        public virtual TTo ConvertFrom(TFrom from) => default;
        public virtual TFrom ConvertTo(TTo to) => default;
    }
}
