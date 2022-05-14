namespace System.Windows.Forms
{
    public class GenericPropertyGrid<TObject> : PropertyGrid, IObjectControl<TObject>
    {
        public TObject Value
        {
            get => (TObject)base.SelectedObject;
            set => base.SelectedObject = value;
        }

        public bool ReadOnly
        {
            get => !this.Enabled;
            set => this.Enabled = !value;
        }
    }
}
