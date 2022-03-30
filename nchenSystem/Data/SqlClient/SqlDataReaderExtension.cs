using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace System.Data.SqlClient
{
    public static class SqlDataReaderExtension
    {
        internal static List<PropertyInfo> GetProperties(this SqlDataReader reader, GetPropertyFunction getProperty)
        {
            var toReturn = new List<PropertyInfo>();
            for (int index = 0; index < reader.FieldCount; index++)
            {
                var columnName = reader.GetName(index);
                var property = getProperty(columnName);
                toReturn.Add(property);
            }
            return toReturn;
        }

        /// <summary>
        /// Read Object
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="reader"></param>
        /// <param name="propertyInfos">List of property</param>
        /// <returns>Return new <typeparamref name="TObject"/> if read successfully, otherwise return <see langword="default"/>.</returns>
        public static TObject ReadObject<TObject>(this SqlDataReader reader, IList<PropertyInfo> propertyInfos)
            where TObject : new()
        {
            if (!reader.Read()) return default;
            return reader.GetObject<TObject>(propertyInfos);
        }

        public static async Task<TObject> ReadObjectAsync<TObject>(this SqlDataReader reader, IList<PropertyInfo> propertyInfos)
            where TObject : new()
        {
            if (!await reader.ReadAsync()) return default;
            return reader.GetObject<TObject>(propertyInfos);
        }

        public static TObject GetObject<TObject>(this SqlDataReader reader, IList<PropertyInfo> propertyInfos)
            where TObject : new()
        {
            var @object = new TObject();
            for (int index = 0; index < reader.FieldCount; index++)
            {
                var value = reader.GetValue(index);
                if (value == Convert.DBNull) value = null;
                propertyInfos[index]?.SetValue(@object, value);
            }
            return @object;
        }
    }
}
