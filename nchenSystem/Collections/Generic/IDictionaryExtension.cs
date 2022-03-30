using System.IO.Types;

namespace System.Collections.Generic
{
    public static class IDictionaryExtension
    {
        /// <summary>
        /// Update object in the dictionary if it exist
        /// </summary>
        /// <typeparam name="TKey">Key</typeparam>
        /// <typeparam name="TObject">Value</typeparam>
        /// <param name="dictionary">Dictionary</param>
        /// <param name="fields">array of fields to be updated to the object</param>
        /// <param name="columnInfos">column information to be used to read fields</param>
        /// <param name="keyColumn">column info that to be used for key</param>
        /// <returns>return true if found and updated successfully otherwise false</returns>
        public static bool UpdateObject<TKey, TObject>(this IDictionary<TKey, TObject> dictionary, string[] fields, IEnumerable<DelimitedFileColumnInfo> columnInfos, DelimitedFileColumnInfo keyColumn)
        {
            var key = (TKey)Convert.ChangeType(fields[keyColumn.Index.Value], typeof(TKey));
            if (!dictionary.TryGetValue(key, out var item))
                return false;
            item.UpdateObject(columnInfos, fields);
            //foreach (var columnInfo in columnInfos)
            //{
            //    var converter = columnInfo.Property.GetTypeConverter();
            //    var value = converter.ConvertFrom(fields[columnInfo.Index.Value]);
            //    columnInfo.Property.SetValue(item, value);
            //}
            return true;
        }
    }
}
