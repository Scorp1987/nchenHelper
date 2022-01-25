using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Attributes;
using System.Data.Types;
using System.Linq;
using System.Reflection;

namespace System.Data
{
    public class DataRow<TObject> : DataRow
        where TObject : class, INotifyPropertyChanged, new()
    {
        private TObject _values;
        /// <summary>
        /// The object of <typeparamref name="TObject"/> type for current <see cref="DataRow{TObject}"/>.
        /// </summary>
        public TObject Values
        {
            get
            {
                if (_values == null)
                {
                    this._values = new TObject();
                    foreach (DataColumn column in base.Table.Columns)
                        UpdateObjValue(column.ColumnName, base[column.ColumnName]);
                    SetPropertyChangedHandler();
                    //_values.PropertyChanged += this.HandlePropertyChanged;
                }
                return _values;
            }
            set
            {
                //if (_values != null)
                //    _values.PropertyChanged -= this.HandlePropertyChanged;
                UnsetPropertyChangedHandler();
                _values = value;
                foreach (var info in this.Infos.Values)
                    UpdateBaseRowValue(info);
                SetPropertyChangedHandler();
                //if (_values != null)
                //    _values.PropertyChanged += this.HandlePropertyChanged;
            }
        }

        /// <summary>
        /// The property and attribute information with ColumnName as key
        /// </summary>
        private Dictionary<string, PropertyAttributeInfo> Infos { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRow{TObject}"/>.
        /// Constructs a row from the builder. Only for internal usage.
        /// </summary>
        /// <param name="builder">builder</param>
        protected internal DataRow(DataRowBuilder builder) : base(builder)
        {
            var query = GetInfos();
            this.Infos = query.ToDictionary(i => i.GetColumnName());
        }

        /// <summary>
        /// Get or Set both underlying value of <see cref="DataRow{TObject}"/> for given <paramref name="column"/>
        /// and object of <typeparamref name="TObject"/> type for current <see cref="DataRow{TObject}"/>
        /// </summary>
        /// <param name="column"></param>
        /// <returns>
        /// underlying value of <see cref="DataRow{TObject}"/> for given <paramref name="column"/>
        /// </returns>
        public new object this[DataColumn column]
        {
            get => GetBaseRowValue(column.ColumnName);
            set => UpdateObjValue(column.ColumnName, value);
        }

        /// <summary>
        /// Get or Set both underlying value of <see cref="DataRow{TObject}"/> for given <paramref name="index"/>
        /// and object of <typeparamref name="TObject"/> type for current <see cref="DataRow{TObject}"/>
        /// </summary>
        /// <param name="index">The zero-based index of the column</param>
        /// <returns>underlying value of <see cref="DataRow{TObject}"/> for given <paramref name="index"/></returns>
        public new object this[int index]
        {
            get => this[base.Table.Columns[index]];
            set => this[base.Table.Columns[index]] = value;
        }

        /// <summary>
        /// Get or Set both underlying value of <see cref="DataRow{TObject}"/>
        /// for given <see cref="DataColumn"/> that is associated with property with <paramref name="propertyName"/> of <typeparamref name="TObject"/>
        /// and object of <typeparamref name="TObject"/> type for current <see cref="DataRow{TObject}"/>
        /// </summary>
        /// <param name="propertyName"><see cref="MemberInfo.Name"/> of property of <typeparamref name="TObject"/></param>
        /// <returns>underlying value of <see cref="DataRow{TObject}"/> for given <paramref name="index"/></returns>
        public new object this[string propertyName]
        {
            get => GetBaseRowValue(this.GetColumnName(propertyName));
            set => UpdateObjValue(this.GetColumnName(propertyName), value);
        }

        /// <summary>
        /// Return the name of the Column that is associated with property with <paramref name="propertyName"/> of <typeparamref name="TObject"/>
        /// </summary>
        /// <param name="propertyName"><see cref="MemberInfo.Name"/> of property of <typeparamref name="TObject"/></param>
        /// <returns>underlying value of <see cref="DataRow{TObject}"/> for given <paramref name="index"/></returns>
        protected virtual string GetColumnName(string propertyName) => propertyName;

        private void SetPropertyChangedHandler()
        {
            if (_values != null) _values.PropertyChanged += this.HandlePropertyChanged;
        }

        internal void UnsetPropertyChangedHandler()
        {
            if (_values != null) _values.PropertyChanged -= this.HandlePropertyChanged;
        }

        /// <summary>
        /// Get collection of <see cref="PropertyInfo"/> with
        /// <see cref="DataColumnInfoAttribute"/> from <typeparamref name="TObject"/> type
        /// </summary>
        /// <returns>
        /// Collection of <see cref="PropertyAttributeInfo"/>
        /// from <typeparamref name="TObject"/> type.
        /// When property doesn't have <see cref="DataColumnInfoAttribute"/>,
        /// <see cref="PropertyAttributeInfo.Attribute"/> will be <see langword="null"/>.
        /// When property have multiple <see cref="DataColumnInfoAttribute"/>,
        /// <see cref="PropertyAttributeInfo.Attribute"/> will be the first <see cref="DataColumnInfoAttribute"/>found.
        /// </returns>
        internal virtual IEnumerable<PropertyAttributeInfo> GetInfos() =>
            from property in typeof(TObject).GetProperties()
            select new PropertyAttributeInfo { Property = property };

        /// <summary>
        /// Update underlying value of <see cref="DataRow{TObject}"/>
        /// for given <paramref name="info"/>
        /// </summary>
        /// <param name="info">column Information</param>
        /// <exception cref="StrongTypingException">
        /// When <see cref="DataColumn.AllowDBNull"/> is <see langword="false"/>
        /// for <see cref="DataColumn"/> that have <paramref name="columnName"/>
        /// and <paramref name="value"/> is <see langword="null"/>
        /// </exception>
        private void UpdateBaseRowValue(PropertyAttributeInfo info)
        {
            var columnName = info.GetColumnName();

            object value = null;
            if (_values != null)
                value = info.Property.GetValue(_values);

            UpdateBaseRowValue(columnName, value);
        }

        /// <summary>
        /// Update underlying value of <see cref="DataRow{TObject}"/>
        /// for given <paramref name="columnName"/>
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="value">value to update</param>
        /// <exception cref="StrongTypingException">
        /// When <see cref="DataColumn.AllowDBNull"/> is <see langword="false"/>
        /// for <see cref="DataColumn"/> that have <paramref name="columnName"/>
        /// and <paramref name="value"/> is <see langword="null"/>
        /// </exception>
        private void UpdateBaseRowValue(string columnName, object value)
        {
            var column = base.Table.Columns[columnName];

            if (value == null)
                if (column.AllowDBNull)
                    base[columnName] = Convert.DBNull;
                else
                    throw new StrongTypingException($"The value for column '{columnName}' in table '{base.Table.TableName}' is DBNull.");
            else
                base[columnName] = value;
        }

        /// <summary>
        /// Get underlying value of <see cref="DataRow{TObject}"/>
        /// for given <paramref name="columnName"/>
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>
        /// underlying value of <see cref="DataRow{TObject}"/>.
        /// <see langword="null"/> if value is <see cref="Convert.DBNull"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// The <see cref="DataColumn"/> specified by <paramref name="columnName"/> cannot be found.
        /// </exception>
        private object GetBaseRowValue(string columnName) => (base[columnName] == Convert.DBNull) ? null : base[columnName];

        /// <summary>
        /// Update value of property of <see cref="Values"/>
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <param name="value">value to set to property</param>
        /// <exception cref="ArgumentNullException">
        /// columnName is null.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// column information specified by <paramref name="columnName"/> cannot be found.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// The property of <typeparamref name="TObject"/> set accessor is not found.
        /// -or- value cannot be converted to the type of <see cref="PropertyInfo.PropertyType"/>
        /// </exception>
        /// <exception cref="TargetException">
        /// The type of obj does not match the target type,
        /// or a property is an instance property but obj is null.
        /// </exception>
        /// <exception cref="MethodAccessException">
        /// There was an illegal attempt to access a private or protected method inside a class.
        /// </exception>
        /// <exception cref="TargetInvocationException">
        /// An error occurred while setting the property value.
        /// <see cref="Exception.InnerException"/> property indicates the reason for the error.
        /// </exception>
        internal void UpdateObjValue(string columnName, object value)
        {
            var info = this.Infos[columnName];
            if (value == Convert.DBNull) value = null;
            info.Property.SetValue(this.Values, value);
        }

        private void HandlePropertyChanged(object obj, PropertyChangedEventArgs e)
        {
            var info = (from pinfo in this.Infos.Values
                        where pinfo.Property.Name == e.PropertyName
                        select pinfo).FirstOrDefault();
            UpdateBaseRowValue(info);
        }
    }

    public class DataRow<TObject, TAttribute> : DataRow<TObject>
        where TObject : class, INotifyPropertyChanged, new()
        where TAttribute : DataColumnInfoAttribute
    {
        public DataRow(DataRowBuilder builder) : base(builder) { }

        protected override string GetColumnName(string propertyName) => typeof(TObject).GetDataTableColumnName<TAttribute>(propertyName);

        internal override IEnumerable<PropertyAttributeInfo> GetInfos() =>
            from property in typeof(TObject).GetProperties()
            let attribute = property.GetCustomAttributes<TAttribute>().FirstOrDefault()
            where attribute != null
            select new PropertyAttributeInfo
            {
                Property = property,
                Attribute = attribute
            };
    }
}
