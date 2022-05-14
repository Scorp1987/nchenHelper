using System.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Data.SqlClient
{
    public static class SqlConnectionExtension
    {
        /// <summary>
        /// Effienctly copies all rows in the supplied
        /// <see cref="DataTable"/> to a destination table
        /// specified by the <paramref name="tableName"/>
        /// and <paramref name="columnNames"/> parameters
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        public static void BulkWriteToServer(this SqlConnection conn, DataTable sourceTable, string tableName, IEnumerable<string> columnNames)
        {
            using (var objBulk = new SqlBulkCopy(conn) { DestinationTableName = tableName })
            {
                objBulk.MapColumns(columnNames);
                objBulk.WriteToServer(sourceTable);
                objBulk.Close();
            }
        }
        /// <summary>
        /// Effienctly copies all rows in the supplied
        /// <see cref="DataTable"/> to a destination table
        /// specified by the <paramref name="tableName"/>
        /// and <paramref name="columnNames"/> parameters
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        public static async Task BulkWriteToServerAsync(this SqlConnection conn, DataTable sourceTable, string tableName, IEnumerable<string> columnNames)
        {
            using (var objBulk = new SqlBulkCopy(conn) { DestinationTableName = tableName })
            {
                objBulk.MapColumns(columnNames);
                await objBulk.WriteToServerAsync(sourceTable);
                objBulk.Close();
            }
        }

        /// <summary>
        /// Effienctly copies all rows in the supplied
        /// <see cref="DataTable"/> to a destination table
        /// specified by the <paramref name="tableName"/> parameters
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        public static void BulkWriteToServer(this SqlConnection conn, DataTable sourceTable, string tableName)
        {
            var columnNames = sourceTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            conn.BulkWriteToServer(sourceTable, tableName, columnNames);
        }
        /// <summary>
        /// Effienctly copies all rows in the supplied
        /// <see cref="DataTable"/> to a destination table
        /// specified by the <paramref name="tableName"/> parameters
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        public static async Task BulkWriteToServerAsync(this SqlConnection conn, DataTable sourceTable, string tableName)
        {
            var columnNames = sourceTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            await conn.BulkWriteToServerAsync(sourceTable, tableName, columnNames);
        }


        /// <summary>
        /// Effienctly copies all items in the supplied
        /// <see cref="IEnumerable{T}"/> to a destination table
        /// specified by the <paramref name="tableName"/> parameters
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="conn"></param>
        /// <param name="collection"></param>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        public static void BulkWriteToServer<TObject>(this SqlConnection conn, IEnumerable<TObject> collection, string tableName, IEnumerable<string> columnNames)
        {
            using(var table = new DataTable())
            {
                table.AddColumns<TObject>(columnNames, out var properties);
                table.AddDataRows(collection, properties);
                conn.BulkWriteToServer(table, tableName);
            }
        }
        public static async Task BulkWriteToServerAsync<TObject>(this SqlConnection conn, IEnumerable<TObject> collection, string tableName, IEnumerable<string> columnNames)
        {
            using (var table = new DataTable())
            {
                table.AddColumns<TObject>(columnNames, out var properties);
                table.AddDataRows(collection, properties);
                await conn.BulkWriteToServerAsync(table, tableName);
            }
        }

        public static void BulkWriteToServer<TObject>(this SqlConnection conn, IEnumerable<TObject> collection, string tableName)
        {
            var columnNames = typeof(TObject).GetNames();
            conn.BulkWriteToServer(collection, tableName, columnNames);
        }
        public static async Task BulkWriteToServerAsync<TObject>(this SqlConnection conn, IEnumerable<TObject> collection, string tableName)
        {
            var columnNames = typeof(TObject).GetNames();
            await conn.BulkWriteToServerAsync(collection, tableName, columnNames);
        }

        public static void BulkWriteToServer<TObject, TAttribute>(this SqlConnection conn, IEnumerable<TObject> collection, string tableName, IEnumerable<string> columnNames)
            where TAttribute : NamedAttribute
        {
            using (var table = new DataTable())
            {
                table.AddColumns<TObject, TAttribute>(columnNames, out var properties);
                table.AddDataRows(collection, properties);
                conn.BulkWriteToServer(table, tableName);
            }
        }
        public static async Task BulkWriteToServerAsync<TObject, TAttribute>(this SqlConnection conn, IEnumerable<TObject> collection, string tableName, IEnumerable<string> columnNames)
            where TAttribute : NamedAttribute
        {
            using (var table = new DataTable())
            {
                table.AddColumns<TObject, TAttribute>(columnNames, out var properties);
                table.AddDataRows(collection, properties);
                await conn.BulkWriteToServerAsync(table, tableName);
            }
        }

        public static void BulkWriteToServer<TObject, TAttribute>(this SqlConnection conn, IEnumerable<TObject> collection, string tableName)
            where TAttribute : NamedAttribute
        {
            var columnNames = typeof(TObject).GetNames<TAttribute>();
            conn.BulkWriteToServer<TObject, TAttribute>(collection, tableName, columnNames);
        }
        public static async Task BulkWriteToServerAsync<TObject, TAttribute>(this SqlConnection conn, IEnumerable<TObject> collection, string tableName)
            where TAttribute : NamedAttribute
        {
            var columnNames = typeof(TObject).GetNames<TAttribute>();
            await conn.BulkWriteToServerAsync<TObject, TAttribute>(collection, tableName, columnNames);
        }
    }
}
