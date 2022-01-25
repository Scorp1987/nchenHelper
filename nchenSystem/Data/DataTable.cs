using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Attributes;
using System.Data.SqlClient;
using System.Data.Types;
using System.Linq;
using System.Reflection;

namespace System.Data
{
    public class DataTable<TObject> : DataTable
        where TObject : class, INotifyPropertyChanged, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTable{TObject}"/> class
        /// with specified table name.
        /// </summary>
        /// <param name="tableName">
        /// The name to give the table.
        /// If tableName is null or an empty string,
        /// a default name is given when added to the <see cref="DataTableCollection"/>.
        /// </param>
        public DataTable(string tableName) : base()
        {
            TableName = tableName;
            BeginInit();
            InitClass();
            EndInit();
        }
        /// <summary>
        /// Initializes a new instance of the System.Data.DataTable class with no arguments.
        /// </summary>
        public DataTable() : this(typeof(TObject).Name) { }

        /// <summary>
        /// Occurs when a <see cref="DataRow{TObject}"/> is changing.
        /// </summary>
        public event EventHandler<DataRowChangeEventArgs<TObject>> DataRowChanging;
        /// <summary>
        /// Occurs after a <see cref="DataRow{TObject}"/> has been changed successfully.
        /// </summary>
        public event EventHandler<DataRowChangeEventArgs<TObject>> DataRowChanged;
        /// <summary>
        /// Occurs before a row in the table is about to be deleted.
        /// </summary>
        public event EventHandler<DataRowChangeEventArgs<TObject>> DataRowDeleting;
        /// <summary>
        /// Occurs after a row in the table has been deleted.
        /// </summary>
        public event EventHandler<DataRowChangeEventArgs<TObject>> DataRowDeleted;

        /// <summary>
        /// Gets the total number of <see cref="DataRow{TObject}"/> objects in this table.
        /// </summary>
        /// <Returns>The total number of <see cref="DataRow{TObject}"/> objects in this table.</Returns>
        public int Count => Rows.Count;

        /// <summary>
        /// Get the row at the specified index
        /// </summary>
        /// <param name="index">The zero-based index of the row to return</param>
        /// <returns>The specified <see cref="DataRow{TObject}"/></returns>
        /// <exception cref="IndexOutOfRangeException">
        /// The index value is greater than the number of items in the collection.
        /// </exception>
        public DataRow<TObject> this[int index] => (DataRow<TObject>)this.Rows[index];

        /// <summary>
        /// Adds the specified <see cref="DataRow{TObject}"/> to this table.
        /// </summary>
        /// <param name="row">The <see cref="DataRow{TObject}"/> to add.</param>
        /// <exception cref="ArgumentNullException">
        /// The row is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The row either belongs to another table or already belongs to this table.
        /// </exception>
        /// <exception cref="ConstraintException">
        /// The addition invalidates a constraint.
        /// </exception>
        /// <exception cref="NoNullAllowedException">
        /// The addition tries to put a null in a <see cref="DataColumn"/>
        /// where <see cref="DataColumn.AllowDBNull"/> is false.
        /// </exception>
        public void AddDataRow(DataRow<TObject> row) => this.Rows.Add(row);
        /// <summary>
        /// Create new <see cref="DataRow{TObject}"/> with specified object with <typeparamref name="TObject"/> then adds to this table.
        /// </summary>
        /// <param name="values">object of <typeparamref name="TObject"/> type</param>
        /// <exception cref="ArgumentNullException">
        /// The row is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The row either belongs to another table or already belongs to this table.
        /// </exception>
        /// <exception cref="ConstraintException">
        /// The addition invalidates a constraint.
        /// </exception>
        /// <exception cref="NoNullAllowedException">
        /// The addition tries to put a null in a <see cref="DataColumn"/>
        /// where <see cref="DataColumn.AllowDBNull"/> is false.
        /// </exception>
        public void AddDataRow(TObject values)
        {
            var row = NewRow();
            row.Values = values;
            AddDataRow(row);
        }


        /// <summary>
        /// Create new <see cref="DataRow{TObject}"/> with specified object with <typeparamref name="TObject"/> then adds to this table.
        /// </summary>
        /// <param name="objects">objects of <typeparamref name="TObject"/> type</param>
        /// <exception cref="ArgumentNullException">
        /// The row is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The row either belongs to another table or already belongs to this table.
        /// </exception>
        /// <exception cref="ConstraintException">
        /// The addition invalidates a constraint.
        /// </exception>
        /// <exception cref="NoNullAllowedException">
        /// The addition tries to put a null in a <see cref="DataColumn"/>
        /// where <see cref="DataColumn.AllowDBNull"/> is false.
        /// </exception>
        public void AddDataRows(IEnumerable<TObject> objects)
        {
            foreach (var @object in objects)
                this.AddDataRow(@object);
        }

        /// <summary>
        /// Create a <see cref="List{TObject}"/> from collection of <see cref="DataRow{TObject}"/> of this table
        /// </summary>
        /// <returns><see cref="List{TObject}"/> from collection of <see cref="DataRow{TObject}"/> of this table</returns>
        public List<TObject> ToList()
        {
            var query = from DataRow<TObject> row in this.Rows
                        select row.Values;
            return query.ToList();
        }

        /// <summary>
        /// Add to specified <see cref="ICollection{T}"/> from collection of <see cref="DataRow{TObject}"/> of this table
        /// </summary>
        /// <param name="collection">collection to add item.</param>
        public void AddToCollection(ICollection<TObject> collection)
        {
            foreach (DataRow<TObject> row in this.Rows)
                collection.Add(row.Values);
        }

        /// <summary>
        /// Effienctly copies all rows in the supplied
        /// <see cref="DataTable{TObject}"/> to a destination table
        /// </summary>
        /// <param name="conn"><see cref="SqlConnection"/> object</param>
        /// <param name="tableName">name of the table in the server</param>
        public virtual void BulkWriteToServer(SqlConnection conn, string tableName)
        {
            var columnNames = typeof(TObject).GetProperties().Select(p => p.Name); // ObjectExtensions.GetColumnNames<TObject>();
            this.BulkWriteToServer(conn, tableName, columnNames);
        }

        /// <summary>
        /// Creates a new instance of <see cref="DataRow{TObject}"/>.
        /// </summary>
        /// <returns>The new expression.</returns>
        protected override DataTable CreateInstance() => new DataTable<TObject>();

        /// <summary>
        /// Dispose this table
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            RowChanged -= HandleRowChanged;
            RowChanging -= HandleRowChanging;
            RowDeleted -= HandleRowDeleted;
            RowDeleting -= HandleRowDeleting;

            this.Rows.Cast<DataRow<TObject>>().AsParallel().ForAll(row => row.UnsetPropertyChangedHandler());

            base.Dispose(disposing);
        }

        internal virtual IEnumerable<PropertyAttributeInfo> GetInfos() =>
            from property in typeof(TObject).GetProperties()
            let attribute = property.GetCustomAttributes<DataColumnInfoAttribute>().FirstOrDefault()
            select new PropertyAttributeInfo
            {
                Property = property,
                Attribute = attribute
            };

        private void InitClass()
        {
            var query = GetInfos();

            foreach (var info in query)
            {
                var columnName = info.GetColumnName();
                var expression = info.GetExpression();
                var mappingType = info.GetMappingType();
                var converter = info.GetConverter();
                Type type = info.GetDataType(converter);
                bool allowDbNull = info.GetAllowDBNull(converter);
                var column = new DataColumn(columnName, type, expression, mappingType) { AllowDBNull = allowDbNull };
                Columns.Add(column);

                RowChanged += HandleRowChanged;
                RowChanging += HandleRowChanging;
                RowDeleted += HandleRowDeleted;
                RowDeleting += HandleRowDeleting;
            }
        }

        /// <summary>
        /// Creates a new <see cref="DataRow{TObject}"/> with the same schema as the table.
        /// </summary>
        /// <returns>
        /// A <see cref="DataRow{TObject}"/> with the same schema as the <see cref="DataTable{TObject}"/>.
        /// </returns>
        public new DataRow<TObject> NewRow() => (DataRow<TObject>)base.NewRow();
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder) => new DataRow<TObject>(builder);

        protected override Type GetRowType() => typeof(DataRow<TObject>);

        protected override void OnRowChanged(DataRowChangeEventArgs e) => base.OnRowChanged(e);
        protected override void OnRowChanging(DataRowChangeEventArgs e) => base.OnRowChanging(e);
        protected override void OnRowDeleted(DataRowChangeEventArgs e) => base.OnRowDeleted(e);
        protected override void OnRowDeleting(DataRowChangeEventArgs e) => base.OnRowDeleting(e);

        private void HandleRowChanged(object sender, DataRowChangeEventArgs e) => DataRowChanged?.Invoke(this, new DataRowChangeEventArgs<TObject>((DataRow<TObject>)e.Row, e.Action));
        private void HandleRowChanging(object sender, DataRowChangeEventArgs e) => DataRowChanging?.Invoke(this, new DataRowChangeEventArgs<TObject>((DataRow<TObject>)e.Row, e.Action));
        private void HandleRowDeleted(object sender, DataRowChangeEventArgs e) => DataRowDeleted?.Invoke(this, new DataRowChangeEventArgs<TObject>((DataRow<TObject>)e.Row, e.Action));
        private void HandleRowDeleting(object sender, DataRowChangeEventArgs e) => DataRowDeleting?.Invoke(this, new DataRowChangeEventArgs<TObject>((DataRow<TObject>)e.Row, e.Action));

        public void RemoveDataRow(DataRow<TObject> row) => Rows.Remove(row);
    }

    public class DataTable<TObject, TAttribute> : DataTable<TObject>
        where TObject : class, INotifyPropertyChanged, new()
        where TAttribute : DataColumnInfoAttribute
    {
        public DataTable(string name) : base(name) { }
        public DataTable() : base() { }

        public new DataRow<TObject, TAttribute> this[int index] => (DataRow<TObject, TAttribute>)this.Rows[index];

        public void AddDataRow(DataRow<TObject, TAttribute> row) => this.Rows.Add(row);

        public override void BulkWriteToServer(SqlConnection conn, string tableName)
        {
            var columnNames = typeof(TObject).GetDataTableColumnNames<TAttribute>();
            this.BulkWriteToServer(conn, tableName, columnNames);
        }

        protected override DataTable CreateInstance() => new DataTable<TObject, TAttribute>();

        internal override IEnumerable<PropertyAttributeInfo> GetInfos() =>
            from property in typeof(TObject).GetProperties()
            let attribute = property.GetCustomAttributes<TAttribute>().FirstOrDefault()
            where attribute != null
            select new PropertyAttributeInfo
            {
                Property = property,
                Attribute = attribute
            };

        public new DataRow<TObject, TAttribute> NewRow() => (DataRow<TObject, TAttribute>)base.NewRow();
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder) => new DataRow<TObject, TAttribute>(builder);

        protected override Type GetRowType() => typeof(DataRow<TObject, TAttribute>);
    }
}
