using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace System.Data
{
    public static class DataTableExtension
    {
        /// <summary>
        /// Effienctly copies all rows in the supplied
        /// <see cref="DataTable"/> to a destination table
        /// specified by the <paramref name="tableName"/>
        /// and <paramref name="columnNames"/> parameters
        /// </summary>
        /// <param name="table"></param>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        public static void BulkWriteToServer(this DataTable table, SqlConnection conn, string tableName, IEnumerable<string> columnNames)
        {
            using (var objBulk = new SqlBulkCopy(conn) { DestinationTableName = tableName })
            {
                objBulk.MapColumns(columnNames);
                objBulk.WriteToServer(table);
                objBulk.Close();
            }
        }

        /// <summary>
        /// Effienctly copies all rows in the supplied
        /// <see cref="DataTable"/> to a destination table
        /// specified by the <paramref name="tableName"/>
        /// and <paramref name="columnNames"/> parameters
        /// </summary>
        /// <param name="table"></param>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        public static void BulkWriteToServer(this DataTable table, SqlConnection conn, string tableName)
        {
            var columnNames = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            table.BulkWriteToServer(conn, tableName, columnNames);
        }
    }
}
