using System.Collections.Generic;
using System.Data.Attributes;
using System.Reflection;
using System.Threading.Tasks;

namespace System.Data.SqlClient
{
    internal delegate PropertyInfo GetPropertyFunction(string columnName);

    public static class SqlCommandExtension
    {
        private static T ExecuteNonQuery<T>(this SqlCommand command, string commandText, Func<T> execute)
        {
            command.CommandText = commandText;
            command.Parameters.Clear();
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


        private static List<TObject> ReadObjects<TObject>(this SqlCommand command, GetPropertyFunction getProperty)
            where TObject : new()
        {
            var toReturn = new List<TObject>();
            using (var reader = command.ExecuteReader())
            {
                var propertyInfos = reader.GetProperties(getProperty);
                while (reader.Read())
                {
                    var @object = reader.GetObject<TObject>(propertyInfos);
                    toReturn.Add(@object);
                }
            }
            return toReturn;
        }
        public static List<TObject> ReadObjects<TObject>(this SqlCommand command)
            where TObject : new()
             => command.ReadObjects<TObject>(columnName => typeof(TObject).GetProperty(columnName));
        public static List<TObject> ReadObjects<TObject, TAttribute>(this SqlCommand command)
            where TObject : new()
            where TAttribute : DataColumnInfoAttribute
            => command.ReadObjects<TObject>(columnName => typeof(TObject).GetDataTableProperty<TAttribute>(columnName));


        private static async Task<List<TObject>> ReadObjectsAsync<TObject>(this SqlCommand command, GetPropertyFunction getProperty)
            where TObject : new()
        {
            var toReturn = new List<TObject>();
            using (var reader = await command.ExecuteReaderAsync())
            {
                var propertyInfos = reader.GetProperties(getProperty);
                while (await reader.ReadAsync())
                {
                    var @object = reader.GetObject<TObject>(propertyInfos);
                    toReturn.Add(@object);
                }
            }
            return toReturn;
        }
        public static Task<List<TObject>> ReadObjectsAsync<TObject>(this SqlCommand command)
            where TObject : new()
            => command.ReadObjectsAsync<TObject>(columnName => typeof(TObject).GetProperty(columnName));
        public static Task<List<TObject>> ReadObjectsAsync<TObject, TAttribute>(this SqlCommand command)
            where TObject : new()
            where TAttribute : DataColumnInfoAttribute
            => command.ReadObjectsAsync<TObject>(columnName => typeof(TObject).GetDataTableProperty<TAttribute>(columnName));


        private static TObject ReadObject<TObject>(this SqlCommand command, GetPropertyFunction getProperty)
            where TObject : new()
        {
            using (var reader = command.ExecuteReader())
            {
                var propertyInfos = reader.GetProperties(getProperty);
                return reader.ReadObject<TObject>(propertyInfos);
            }
        }
        public static TObject ReadObject<TObject>(this SqlCommand command)
            where TObject : new()
            => command.ReadObject<TObject>(columnName => typeof(TObject).GetProperty(columnName));
        public static TObject ReadObject<TObject, TAttribute>(this SqlCommand command)
            where TObject : new()
            where TAttribute : DataColumnInfoAttribute
            => command.ReadObject<TObject>(columnName => typeof(TObject).GetDataTableProperty<TAttribute>(columnName));

        private static async Task<TObject> ReadObjectAsync<TObject>(this SqlCommand command, GetPropertyFunction getProperty)
            where TObject : new()
        {
            using (var reader = await command.ExecuteReaderAsync())
            {
                var propertyInfos = reader.GetProperties(getProperty);
                return await reader.ReadObjectAsync<TObject>(propertyInfos);
            }
        }
        public static Task<TObject> ReadObjectAsync<TObject>(this SqlCommand command)
            where TObject : new()
            => command.ReadObjectAsync<TObject>(columnName => typeof(TObject).GetProperty(columnName));
        public static Task<TObject> ReadObjectAsync<TObject, TAttribute>(this SqlCommand command)
            where TObject : new()
            where TAttribute : DataColumnInfoAttribute
            => command.ReadObjectAsync<TObject>(columnName => typeof(TObject).GetDataTableProperty<TAttribute>(columnName));
    }
}
