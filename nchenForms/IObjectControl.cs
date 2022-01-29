namespace System.Windows.Forms
{
    public interface IObjectControl<TObject> : IDataControl
    {
        TObject Value { get; set; }
    }
}
