using System.ComponentModel;

namespace System.Data
{
    public class DataRowChangeEventArgs<TObject> : DataRowChangeEventArgs
        where TObject : INotifyPropertyChanging, new()
    {
        public DataRowChangeEventArgs(DataRow<TObject> row, DataRowAction action) : base(row, action) { }
    }
}
