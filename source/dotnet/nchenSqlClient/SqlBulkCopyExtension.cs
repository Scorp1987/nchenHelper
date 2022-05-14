using System.Collections.Generic;

namespace System.Data.SqlClient
{
    public static class SqlBulkCopyExtension
    {
        /// <summary>
        /// Creates multiple new <see cref="SqlBulkCopyColumnMapping"/> and adds it to the
        /// collection, using <paramref name="columnNames"/> specify both source and destination columns.
        /// </summary>
        /// <param name="objBulk"></param>
        /// <param name="columnNames">the collection of the name of source and destination columns</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="columnNames"/> is null
        /// </exception>
        public static void MapColumns(this SqlBulkCopy objBulk, IEnumerable<string> columnNames)
        {
            if (columnNames == null) throw new ArgumentNullException(nameof(columnNames));
            foreach (var columnName in columnNames)
                objBulk.ColumnMappings.Add(columnName, columnName);
        }
    }
}
