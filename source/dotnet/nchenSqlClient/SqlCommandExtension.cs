using System.Attributes;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace System.Data.SqlClient
{
    public static class SqlCommandExtension
    {
        #region ExecuteNonQuery
        private static T ExecuteNonQuery<T>(this SqlCommand command, string commandText, Func<T> execute)
        {
            command.CommandText = commandText;
            return execute();
        }
        public static int ExecuteNonQuery(this SqlCommand command, string commandText)
            => command.ExecuteNonQuery(commandText, command.ExecuteNonQuery);
        public static Task<int> ExecuteNonQueryAsync(this SqlCommand command, string commandText)
            => command.ExecuteNonQuery(commandText, command.ExecuteNonQueryAsync);

        private static T ExecuteNonQuery<T>(this SqlCommand command, string commandText, Func<T> execute, params SqlParameter[] parameters)
        {
            command.CommandText = commandText;
            command.Parameters.Clear();
            foreach (var parameter in parameters)
                command.Parameters.Add(parameter);
            return execute();
        }
        public static int ExecuteNonQuery(this SqlCommand command, string commandText, params SqlParameter[] parameters)
            => command.ExecuteNonQuery(commandText, command.ExecuteNonQuery, parameters);
        public static Task<int> ExecuteNonQueryAsync(this SqlCommand command, string commandText, params SqlParameter[] parameters)
            => command.ExecuteNonQuery(commandText, command.ExecuteNonQueryAsync, parameters);
        #endregion


        #region Drop Table
        private static string GetDropTableSqlString(string tableName)
            => $"DROP TABLE {tableName};";
        public static int DropTable(this SqlCommand command, string tableName)
            => command.ExecuteNonQuery(GetDropTableSqlString(tableName));
        public static Task<int> DropTableAsync(this SqlCommand command, string tableName)
            => command.ExecuteNonQueryAsync(GetDropTableSqlString(tableName));
        #endregion


        #region Create Table
        private static string GetCreateTableSqlString<TObject>(string tableName)
        {
            var dataStr = "";
            foreach(var property in typeof(TObject).GetProperties())
            {
                var columnName = property.GetName();
                var sqlDataType = property.GetSqlDataType();
                dataStr += $",{columnName} {sqlDataType}";
            }
            dataStr = dataStr[1..];
            return $"CREATE TABLE {tableName} ({dataStr});";
        }
        private static string GetCreateTableSqlString<TObject, TAttribute>(string tableName)
            where TAttribute : DataColumnInfoAttribute
        {
            var dataStr = "";
            foreach (var property in typeof(TObject).GetProperties<TAttribute>())
            {
                var attribute = property.GetCustomAttribute<TAttribute>(true);
                var columnName = property.GetName(attribute);
                var sqlDataType = property.GetSqlDataType(attribute);
                dataStr += $",{columnName} {sqlDataType}";
            }
            dataStr = dataStr[1..];
            return $"CREATE TABLE {tableName} ({dataStr});";
        }

        public static void CreateTable<TObject>(this SqlCommand command, string tableName)
            => command.ExecuteNonQuery(GetCreateTableSqlString<TObject>(tableName));
        public static Task CreateTableAsync<TObject>(this SqlCommand command, string tableName)
            => command.ExecuteNonQueryAsync(GetCreateTableSqlString<TObject>(tableName));

        public static void CreateTable<TObject>(this SqlCommand command, string tableName, IEnumerable<TObject> collection)
        {
            command.CreateTable<TObject>(tableName);
            command.Connection.BulkWriteToServer(collection, tableName);
        }
        public static async Task CreateTableAsync<TObject>(this SqlCommand command, string tableName, IEnumerable<TObject> collection)
        {
            await command.CreateTableAsync<TObject>(tableName);
            await command.Connection.BulkWriteToServerAsync(collection, tableName);
        }

        public static void CreateTable<TObject, TAttribute>(this SqlCommand command, string tableName)
            where TAttribute : DataColumnInfoAttribute
            => command.ExecuteNonQuery(GetCreateTableSqlString<TObject, TAttribute>(tableName));
        public static Task CreateTableAsync<TObject, TAttribute>(this SqlCommand command, string tableName)
            where TAttribute : DataColumnInfoAttribute
            => command.ExecuteNonQueryAsync(GetCreateTableSqlString<TObject, TAttribute>(tableName));

        public static void CreateTable<TObject, TAttribute>(this SqlCommand command, string tableName, IEnumerable<TObject> collection)
            where TAttribute : DataColumnInfoAttribute
        {
            command.CreateTable<TObject, TAttribute>(tableName);
            command.Connection.BulkWriteToServer<TObject, TAttribute>(collection, tableName);
        }
        public static async Task CreateTableAsync<TObject, TAttribute>(this SqlCommand command, string tableName, IEnumerable<TObject> collection)
            where TAttribute : DataColumnInfoAttribute
        {
            await command.CreateTableAsync<TObject, TAttribute>(tableName);
            await command.Connection.BulkWriteToServerAsync<TObject, TAttribute>(collection, tableName);
        }
        #endregion


        #region Read an Object from the first row
        private static TObject ReadObject<TObject>(this SqlCommand command, GetPropertyFunction getProperty)
            where TObject : new()
        {
            using var reader = command.ExecuteReader();
            var propertyInfos = reader.GetProperties(getProperty);
            return reader.ReadObject<TObject>(propertyInfos);
        }
        private static async Task<TObject> ReadObjectAsync<TObject>(this SqlCommand command, GetPropertyFunction getProperty)
            where TObject : new()
        {
            using var reader = await command.ExecuteReaderAsync();
            var propertyInfos = reader.GetProperties(getProperty);
            return await reader.ReadObjectAsync<TObject>(propertyInfos);
        }

        public static TObject ReadObject<TObject>(this SqlCommand command)
            where TObject : new()
            => command.ReadObject<TObject>(columnName => typeof(TObject).GetProperty(columnName));
        public static Task<TObject> ReadObjectAsync<TObject>(this SqlCommand command)
            where TObject : new()
            => command.ReadObjectAsync<TObject>(columnName => typeof(TObject).GetProperty(columnName));

        public static TObject ReadObject<TObject, TAttribute>(this SqlCommand command)
            where TObject : new()
            where TAttribute : NamedAttribute
            => command.ReadObject<TObject>(columnName => typeof(TObject).GetNamedProperty<TAttribute>(columnName));
        public static Task<TObject> ReadObjectAsync<TObject, TAttribute>(this SqlCommand command)
            where TObject : new()
            where TAttribute : NamedAttribute
            => command.ReadObjectAsync<TObject>(columnName => typeof(TObject).GetNamedProperty<TAttribute>(columnName));
        #endregion


        #region Read Objects, Add to Collection
        private static void FillCollection<TObject>(this SqlCommand command, ICollection<TObject> collection, GetPropertyFunction getProperty)
            where TObject : new()
        {
            using var reader = command.ExecuteReader();
            var propertyInfos = reader.GetProperties(getProperty);
            while (reader.Read())
            {
                var @object = reader.GetObject<TObject>(propertyInfos);
                collection.Add(@object);
            }
        }
        private static async Task FillCollectionAsync<TObject>(this SqlCommand command, ICollection<TObject> collection, GetPropertyFunction getProperty)
            where TObject : new()
        {
            using var reader = await command.ExecuteReaderAsync();
            var propertyInfos = reader.GetProperties(getProperty);
            while (await reader.ReadAsync())
            {
                var @object = reader.GetObject<TObject>(propertyInfos);
                collection.Add(@object);
            }
        }

        public static void FillCollection<TObject>(this SqlCommand command, ICollection<TObject> collection)
            where TObject : new()
            => command.FillCollection(collection, columnName => typeof(TObject).GetProperty(columnName));
        public static Task FillCollectionAsync<TObject>(this SqlCommand command, ICollection<TObject> collection)
            where TObject : new()
            => command.FillCollectionAsync(collection, columnName => typeof(TObject).GetProperty(columnName));

        public static void FillCollection<TObject, TAttribute>(this SqlCommand command, ICollection<TObject> collection)
            where TObject : new()
            where TAttribute : NamedAttribute
            => command.FillCollection(collection, columnName => typeof(TObject).GetNamedProperty<TAttribute>(columnName));
        public static Task FillCollectionAsync<TObject, TAttribute>(this SqlCommand command, ICollection<TObject> collection)
            where TObject : new()
            where TAttribute : NamedAttribute
            => command.FillCollectionAsync(collection, columnName => typeof(TObject).GetNamedProperty<TAttribute>(columnName));
        #endregion


        #region Create new Collection, Read Objects, Add To Collection, Return Collection
        public static TCollection ReadCollection<TCollection, TObject>(this SqlCommand command)
            where TCollection : ICollection<TObject>, new()
            where TObject : new()
        {
            var toReturn = new TCollection();
            command.FillCollection(toReturn);
            return toReturn;
        }
        public static async Task<TCollection> ReadCollectionAsync<TCollection, TObject>(this SqlCommand command)
            where TCollection : ICollection<TObject>, new()
            where TObject : new()
        {
            var toReturn = new TCollection();
            await command.FillCollectionAsync(toReturn);
            return toReturn;
        }

        public static TCollection ReadCollection<TCollection, TObject, TAttribute>(this SqlCommand command)
            where TCollection : ICollection<TObject>, new()
            where TObject : new()
            where TAttribute : NamedAttribute
        {
            var toReturn = new TCollection();
            command.FillCollection<TObject, TAttribute>(toReturn);
            return toReturn;
        }
        public static async Task<TCollection> ReadCollectionAsync<TCollection, TObject, TAttribute>(this SqlCommand command)
            where TCollection : ICollection<TObject>, new()
            where TObject : new()
            where TAttribute : NamedAttribute
        {
            var toReturn = new TCollection();
            await command.FillCollectionAsync<TObject, TAttribute>(toReturn);
            return toReturn;
        }
        #endregion


        #region Create new HashSet, Read Objects, Add To Collection, Return HashSet
        public static HashSet<TObject> ReadHashSet<TObject>(this SqlCommand command)
            where TObject : new()
            => command.ReadCollection<HashSet<TObject>, TObject>();
        public static Task<HashSet<TObject>> ReadHashSetAsync<TObject>(this SqlCommand command)
            where TObject : new()
            => command.ReadCollectionAsync<HashSet<TObject>, TObject>();

        public static HashSet<TObject> ReadHashSet<TObject, TAttribute>(this SqlCommand command)
            where TObject : new()
            where TAttribute : NamedAttribute
            => command.ReadCollection<HashSet<TObject>, TObject, TAttribute>();
        public static Task<HashSet<TObject>> ReadHashSetAsync<TObject, TAttribute>(this SqlCommand command)
            where TObject : new()
            where TAttribute : NamedAttribute
            => command.ReadCollectionAsync<HashSet<TObject>, TObject, TAttribute>();
        #endregion


        #region Create new List, Read Objects, Add To Collection, Return List
        public static List<TObject> ReadList<TObject>(this SqlCommand command)
            where TObject : new()
            => command.ReadCollection<List<TObject>, TObject>();
        public static Task<List<TObject>> ReadListAsync<TObject>(this SqlCommand command)
            where TObject : new()
            => command.ReadCollectionAsync<List<TObject>, TObject>();

        public static List<TObject> ReadList<TObject, TAttribute>(this SqlCommand command)
            where TObject : new()
            where TAttribute : NamedAttribute
            => command.ReadCollection<List<TObject>, TObject, TAttribute>();
        public static Task<List<TObject>> ReadListAsync<TObject, TAttribute>(this SqlCommand command)
            where TObject : new()
            where TAttribute : NamedAttribute
            => command.ReadCollectionAsync<List<TObject>, TObject, TAttribute>();
        #endregion


        #region Read Objects, GetKey, Add to Dictionary
        private static void FillDictionary<TKey, TObject>(this SqlCommand command, IDictionary<TKey, TObject> dictionary, Func<TObject, TKey> getKey, GetPropertyFunction getProperty)
            where TObject : new()
        {
            using var reader = command.ExecuteReader();
            var propertyInfos = reader.GetProperties(getProperty);
            while (reader.Read())
            {
                var @object = reader.GetObject<TObject>(propertyInfos);
                var key = getKey(@object);
                dictionary.Add(key, @object);
            }
        }
        private static async Task FillDictionaryAsync<TKey, TObject>(this SqlCommand command, IDictionary<TKey, TObject> dictionary, Func<TObject, TKey> getKey, GetPropertyFunction getProperty)
            where TObject : new()
        {
            using var reader = await command.ExecuteReaderAsync();
            var propertyInfos = reader.GetProperties(getProperty);
            while (await reader.ReadAsync())
            {
                var @object = reader.GetObject<TObject>(propertyInfos);
                var key = getKey(@object);
                dictionary.Add(key, @object);
            }
        }

        public static void FillDictionary<TKey, TObject>(this SqlCommand command, IDictionary<TKey, TObject> dictionary, Func<TObject, TKey> getKey)
            where TObject : new()
            => command.FillDictionary(dictionary, getKey, columnName => typeof(TObject).GetProperty(columnName));
        public static Task FillDictionaryAsync<TKey, TObject>(this SqlCommand command, IDictionary<TKey, TObject> dictionary, Func<TObject, TKey> getKey)
            where TObject : new()
            => command.FillDictionaryAsync(dictionary, getKey, columnName => typeof(TObject).GetProperty(columnName));

        public static void FillDictionary<TKey, TObject, TAttribute>(this SqlCommand command, IDictionary<TKey, TObject> dictionary, Func<TObject, TKey> getKey)
            where TObject : new()
            where TAttribute : NamedAttribute
            => command.FillDictionary(dictionary, getKey, columnName => typeof(TObject).GetNamedProperty<TAttribute>(columnName));
        public static Task FillDictionaryAsync<TKey, TObject, TAttribute>(this SqlCommand command, IDictionary<TKey, TObject> dictionary, Func<TObject, TKey> getKey)
            where TObject : new()
            where TAttribute : NamedAttribute
            => command.FillDictionaryAsync(dictionary, getKey, columnName => typeof(TObject).GetNamedProperty<TAttribute>(columnName));
        #endregion


        #region Read Objects, GetKey, Add to Dictionary, Return Dictionary
        public static Dictionary<TKey, TObject> ReadDictionary<TKey, TObject>(this SqlCommand command, Func<TObject, TKey> getKey)
            where TObject : new()
        {
            var toReturn = new Dictionary<TKey, TObject>();
            command.FillDictionary(toReturn, getKey);
            return toReturn;
        }
        public static async Task<Dictionary<TKey, TObject>> ReadDictionaryAsync<TKey, TObject>(this SqlCommand command, Func<TObject, TKey> getKey)
            where TObject : new()
        {
            var toReturn = new Dictionary<TKey, TObject>();
            await command.FillDictionaryAsync(toReturn, getKey);
            return toReturn;
        }

        public static Dictionary<TKey, TObject> ReadDictionary<TKey, TObject, TAttribute>(this SqlCommand command, Func<TObject, TKey> getKey)
            where TObject : new()
            where TAttribute : NamedAttribute
        {
            var toReturn = new Dictionary<TKey, TObject>();
            command.FillDictionary<TKey, TObject, TAttribute>(toReturn, getKey);
            return toReturn;
        }
        public static async Task<Dictionary<TKey, TObject>> ReadDictionaryAsync<TKey, TObject, TAttribute>(this SqlCommand command, Func<TObject, TKey> getKey)
            where TObject : new()
            where TAttribute : NamedAttribute
        {
            var toReturn = new Dictionary<TKey, TObject>();
            await command.FillDictionaryAsync<TKey, TObject, TAttribute>(toReturn, getKey);
            return toReturn;
        }
        #endregion
    }
}
