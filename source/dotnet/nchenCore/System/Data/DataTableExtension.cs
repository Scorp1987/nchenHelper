using System.Attributes;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data
{
    public static class DataTableExtension
    {
        private static void AddColumns(this DataTable table, IEnumerable<string> columnNames, out Dictionary<string, PropertyInfo> properties, Func<string, PropertyInfo> getProperty)
        {
            properties = new Dictionary<string, PropertyInfo>();
            foreach (var columnName in columnNames)
            {
                table.Columns.Add(columnName);
                var property = getProperty(columnName);
                properties.Add(columnName, property);
            }
        }
        /// <summary>
        /// Add <see cref="DataColumn"/> to the table with the <paramref name="columnNames"/>
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="table"></param>
        /// <param name="columnNames">Name of the columns</param>
        /// <param name="properties">Return the <see cref="Dictionary{TKey, TValue}"/>
        /// where <see langword="TKey"/> is columnName and <see langword="TValue"/> is PropertyInfo related to columnName</param>
        public static void AddColumns<TObject>(this DataTable table, IEnumerable<string> columnNames, out Dictionary<string, PropertyInfo> properties)
            => table.AddColumns(columnNames, out properties, columnName => typeof(TObject).GetProperty(columnName));
        public static void AddColumns<TObject, TAttribute>(this DataTable table, IEnumerable<string> columnNames, out Dictionary<string, PropertyInfo> properties)
            where TAttribute : NamedAttribute
            => table.AddColumns(columnNames, out properties, columnName => typeof(TObject).GetNamedProperty<TAttribute>(columnName));


        public static void AddColumns<TObject>(this DataTable table, out Dictionary<string, PropertyInfo> properties)
        {
            properties = new Dictionary<string, PropertyInfo>();
            foreach (var property in typeof(TObject).GetProperties())
            {
                var columnName = property.Name;
                table.Columns.Add(columnName);
                properties.Add(columnName, property);
            }
        }
        public static void AddColumns<TObject, TAttribute>(this DataTable table, out Dictionary<string, PropertyInfo> properties)
            where TAttribute : NamedAttribute
        {
            properties = new Dictionary<string, PropertyInfo>();
            foreach (var property in typeof(TObject).GetProperties<TAttribute>())
            {
                var columnName = property.GetName<TAttribute>();
                table.Columns.Add(columnName);
                properties.Add(columnName, property);
            }
        }


        public  static void AddDataRows<TObject>(this DataTable table, IEnumerable<TObject> collection, Dictionary<string, PropertyInfo> properties)
        {
            foreach (var @object in collection)
            {
                var row = table.NewRow();
                foreach (var property in properties)
                    row[property.Key] = property.Value.GetValue(@object);
                table.Rows.Add(row);
            }
        }
    }
}
