using Microsoft.VisualBasic.FileIO;
using System;
using System.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualBasic.FileIO
{
    public static class TextFieldParserExtension
    {
        /// <summary>
        /// Get last cursor position of base stream without buffer.
        /// </summary>
        /// <param name="parser"></param>
        /// <returns>return last cursor position of base stream without buffer</returns>
        public static long GetPosition(this TextFieldParser parser)
        {
            parser.PeekChars(1);
            int charRead = (int)parser.GetType().InvokeMember("m_CharsRead", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, parser, null);
            var reader = (StreamReader)typeof(TextFieldParser).InvokeMember("m_Reader", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField, null, parser, null);
            long readerEndPos = reader.GetPosition();
            return readerEndPos - charRead;
        }


        #region Get Column Informations
        /// <summary>
        /// Get Column Information
        /// </summary>
        /// <param name="parser"></param>
        /// <returns>Return Column Information without PropertInfo</returns>
        public static DelimitedFileColumnInfo[] GetColumnInfos(this TextFieldParser parser)
        {
            var columnNames = parser.ReadFields();
            if (columnNames == null) return new DelimitedFileColumnInfo[0];
            var toreturn = new DelimitedFileColumnInfo[columnNames.Length];
            for (var i = 0; i < columnNames.Length; i++)
                toreturn[i] = new DelimitedFileColumnInfo
                {
                    Name = columnNames[i],
                    Index = i
                };
            return toreturn;
        }

        /// <summary>
        /// Get Column Information of all property in object of TObject type
        /// </summary>
        /// <typeparam name="TObject">Object Type</typeparam>
        /// <param name="parser"></param>
        /// <returns>Array of Column Information</returns>
        public static DelimitedFileColumnInfo[] GetColumnInfos<TObject>(this TextFieldParser parser)
        {
            var columnInfos = parser.GetColumnInfos();
            columnInfos.AsParallel().ForAll(columnInfo => columnInfo.Property = typeof(TObject).GetProperty(columnInfo.Name));
            return columnInfos;
            //return (from columnInfo in columnInfos
            //        join property in typeof(TObject).GetProperties()
            //        on columnInfo.Name equals property.GetCustomAttributes<DelimitedFileColumnNameAttribute>().FirstOrDefault()?.Name ?? property.Name into gp1
            //        from property in gp1.DefaultIfEmpty(null)
            //        select new DelimitedFileColumnInfo
            //        {
            //            Name = columnInfo.Name,
            //            Index = columnInfo.Index,
            //            Property = property
            //        }).ToArray();
        }

        /// <summary>
        /// Get Column Informations of property with TAttribute attribute type for object of TObject type
        /// </summary>
        /// <typeparam name="TObject">Object Type</typeparam>
        /// <typeparam name="TAttribute">AttributeType must be DelimitedFileColumnNameAttribute</typeparam>
        /// <param name="parser"></param>
        /// <returns>Array of Column Informations</returns>
        public static DelimitedFileColumnInfo[] GetColumnInfos<TObject, TAttribute>(this TextFieldParser parser)
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            var columnInfos = parser.GetColumnInfos();
            columnInfos.AsParallel().ForAll(columnInfo => columnInfo.Property = typeof(TObject).GetDelimitedFileProperty<TAttribute>(columnInfo));
            return columnInfos;
        }
        #endregion


        #region Read Fields -> Update (or) Read Single Object
        /// <summary>
        /// Update property of the object
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="parser"></param>
        /// <param name="item">destination object</param>
        /// <param name="columnInfos">column informations</param>
        public static void UpdateObject<TObject>(this TextFieldParser parser, TObject item, IEnumerable<DelimitedFileColumnInfo> columnInfos)
        {
            var data = parser.ReadFields();
            foreach (var columnInfo in columnInfos)
            {
                if (columnInfo.Property == null) continue;

                var converter = columnInfo.Property.GetTypeConverter();
                var value = converter.ConvertFrom(data[columnInfo.Index.Value]);
                columnInfo.Property.SetValue(item, value);
            }
        }
        /// <summary>
        /// Update property of dynamic object
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="item">Destination dynamic object</param>
        /// <param name="columnInfos">Column Information without property</param>
        public static void UpdateObject(this TextFieldParser parser, ExpandoObject item, IEnumerable<DelimitedFileColumnInfo> columnInfos)
        {
            var data = parser.ReadFields();
            var dic = item as IDictionary<string, object>;
            foreach (var columnInfo in columnInfos)
            {
                var value = data[columnInfo.Index.Value];
                dic[columnInfo.Name] = value;
            }
        }
        /// <summary>
        /// Create new TObject then update property of the TObject
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="parser"></param>
        /// <param name="columnInfos">column informations</param>
        /// <returns></returns>
        public static TObject ReadObject<TObject>(this TextFieldParser parser, IEnumerable<DelimitedFileColumnInfo> columnInfos)
            where TObject : new()
        {
            var item = new TObject();
            parser.UpdateObject(item, columnInfos);
            return item;
        }
        /// <summary>
        /// Create new dynamic object
        /// Update property of dynamic object
        /// Return dynamic object
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="columnInfos">Column Information without property</param>
        /// <returns>dynamic object</returns>
        public static dynamic ReadDynamic(this TextFieldParser parser, IEnumerable<DelimitedFileColumnInfo> columnInfos)
        {
            var item = new ExpandoObject();
            parser.UpdateObject(item, columnInfos);
            return item;
        }
        #endregion


        #region Read Objects, Add to Collection
        /// <summary>
        /// Create new TObject,
        /// Update property of TObject,
        /// Add TObject to Collection
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="parser"></param>
        /// <param name="collection">Destination Collection</param>
        /// <param name="columnInfos">Column Informations</param>
        public static void AddToCollection<TObject>(this TextFieldParser parser, ICollection<TObject> collection, IEnumerable<DelimitedFileColumnInfo> columnInfos)
            where TObject : new()
        {
            while (!parser.EndOfData)
            {
                var item = new TObject();
                parser.UpdateObject(item, columnInfos);
                collection.Add(item);
            }
        }
        /// <summary>
        /// Create new TObject,
        /// Update property with TAttribute of TObject,
        /// Add TObject to Collection
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="parser"></param>
        /// <param name="collection">Destination Collection</param>
        public static void AddToCollection<TObject>(this TextFieldParser parser, ICollection<TObject> collection)
            where TObject : new()
        {
            var columnInfos = parser.GetColumnInfos<TObject>();
            parser.AddToCollection(collection, columnInfos);
        }
        /// <summary>
        /// Create new TObject,
        /// Update property with TAttribute of TObject,
        /// Add TObject to Collection
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="parser"></param>
        /// <param name="collection">Destination Collection</param>
        public static void AddToCollection<TObject, TAttribute>(this TextFieldParser parser, ICollection<TObject> collection)
            where TObject : new()
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            var columnInfos = parser.GetColumnInfos<TObject, TAttribute>();
            parser.AddToCollection(collection, columnInfos);
        }
        /// <summary>
        /// Create new ExpandoObject,
        /// Update property of ExpandoObject,
        /// Execute getValue to get TValue,
        /// Add TValue to Collection
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="parser"></param>
        /// <param name="collection">Destination Collection</param>
        /// <param name="getValue">Function to get value from dynamic object</param>
        public static void AddToCollection<TValue>(this TextFieldParser parser, ICollection<TValue> collection, Func<dynamic, TValue> getValue)
        {
            var columnInfos = parser.GetColumnInfos();

            while (!parser.EndOfData)
            {
                var item = parser.ReadDynamic(columnInfos);
                var value = (TValue)getValue(item);
                collection.Add(value);
            }
        }
        /// <summary>
        /// Create new ExpandoObject,
        /// Update property of ExpandoObject
        /// Add ExpandoObject to Collection
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="collection">Destination Collection</param>
        public static void AddToCollection(this TextFieldParser parser, ICollection<dynamic> collection)
        {
            var columnInfos = parser.GetColumnInfos();
            while (!parser.EndOfData)
            {
                var item = parser.ReadDynamic(columnInfos);
                collection.Add(item);
            }
        }
        #endregion


        public static void FillTable<TObject>(this TextFieldParser parser, DataTable<TObject> table, IEnumerable<DelimitedFileColumnInfo> columnInfos)
            where TObject : INotifyPropertyChanging, new()
        {
            while (!parser.EndOfData)
            {
                var item = new TObject();
                parser.UpdateObject(item, columnInfos);
                table.AddDataRow(item);
            }
        }
        public static void FillTable<TObject>(this TextFieldParser parser, DataTable<TObject> table)
            where TObject : INotifyPropertyChanging, new()
        {
            var columnInfos = parser.GetColumnInfos<TObject>();
            parser.FillTable(table, columnInfos);
        }
        public static void FillTable<TObject, TAttribute>(this TextFieldParser parser, DataTable<TObject> table)
            where TObject: INotifyPropertyChanging, new()
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            var columnInfos = parser.GetColumnInfos<TObject, TAttribute>();
            parser.FillTable(table, columnInfos);
        }
        public static void FillTable<TValue>(this TextFieldParser parser, DataTable<TValue> table, Func<dynamic, TValue> getValue)
            where TValue : INotifyPropertyChanging, new()
        {
            var columnInfos = parser.GetColumnInfos();

            while (!parser.EndOfData)
            {
                var item = parser.ReadDynamic(columnInfos);
                var value = (TValue)getValue(item);
                table.AddDataRow(value);
            }
        }
        //public static void FillTable(this TextFieldParser parser, DataTable table)
        //{
        //    var columnInfos = parser.GetColumnInfos();
        //    foreach(var column)
        //    while (!parser.EndOfData)
        //    {
        //        var item = parser.ReadDynamic(columnInfos);
        //        collection.Add(item);
        //    }
        //}


        #region Create new Collection, Read Objects, Add To Collection, Return Collection
        /// <summary>
        /// Create new TCollection Collection,
        /// Create new TObject,
        /// Update property of TObject,
        /// Add to TCollection,
        /// Return TCollection
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="parser"></param>
        /// <returns>Return TCollection</returns>
        public static TCollection GetCollection<TCollection, TObject>(this TextFieldParser parser)
            where TCollection : ICollection<TObject>, new()
            where TObject : new()
        {
            var toReturn = new TCollection();
            parser.AddToCollection(toReturn);
            return toReturn;
        }
        /// <summary>
        /// Create new TCollection Collection,
        /// Create new TObject,
        /// Update Property with TAttribute of TObject,
        /// Add to TCollection,
        /// Return TCollection
        /// </summary>
        /// <typeparam name="TCollection"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="parser"></param>
        /// <returns>Rerturn TCollection</returns>
        public static TCollection GetCollection<TCollection, TObject, TAttribute>(this TextFieldParser parser)
            where TCollection : ICollection<TObject>, new()
            where TObject : new()
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            var toreturn = new TCollection();
            parser.AddToCollection<TObject, TAttribute>(toreturn);
            return toreturn;
        }
        #endregion


        #region Create new HashSet, Read Objects, Add To Collection, Return HashSet
        /// <summary>
        /// Create new Hashset,
        /// Create new TObject,
        /// Update Property of TObject,
        /// Add TObject to Hashset,
        /// Return Hashset
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="parser"></param>
        /// <returns>Return Filled Hashset</returns>
        public static HashSet<TObject> GetHashSet<TObject>(this TextFieldParser parser)
            where TObject : new()
        {
            return parser.GetCollection<HashSet<TObject>, TObject>();
        }
        /// <summary>
        /// Create new Hashset,
        /// Create new TObject,
        /// Update Property with TAttribute of TObject,
        /// Add TObject to Hashset,
        /// Return Hashset
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="parser"></param>
        /// <returns>Return Filled Hashset</returns>
        public static HashSet<TObject> GetHashSet<TObject, TAttribute>(this TextFieldParser parser)
            where TObject : new()
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            return parser.GetCollection<HashSet<TObject>, TObject, TAttribute>();
        }
        /// <summary>
        /// Create new Hashset,
        /// Create new ExpandoObject,
        /// Update Property of ExpandoObject,
        /// Execute getValue to get TValue,
        /// Add TValue to Hashset,
        /// Return Hashset
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="parser"></param>
        /// <param name="getValue">Function to get value from dynamic object</param>
        /// <returns>Return Filled Hashset</returns>
        public static HashSet<TValue> GetHashSet<TValue>(this TextFieldParser parser, Func<dynamic, TValue> getValue)
        {
            var toReturn = new HashSet<TValue>();
            parser.AddToCollection(toReturn, getValue);
            return toReturn;
        }
        /// <summary>
        /// Create new Hashset,
        /// Create new ExpandoObject,
        /// Update Property of ExpandoObject,
        /// Add ExpandoObject to Hashset,
        /// Return Hashset
        /// </summary>
        /// <param name="parser"></param>
        /// <returns>Return Filled Hashset</returns>
        public static HashSet<dynamic> GetHashSet(this TextFieldParser parser)
        {
            var toReturn = new HashSet<dynamic>();
            parser.AddToCollection(toReturn);
            return toReturn;
        }
        #endregion


        #region Create new List, Read Objects, Add To Collection, Return List
        /// <summary>
        /// Create new List,
        /// Create new TObject,
        /// Update Property of TObject,
        /// Add TObject to List,
        /// Return List
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="parser"></param>
        /// <returns>Return Filled List</returns>
        public static List<TObject> GetList<TObject>(this TextFieldParser parser)
            where TObject : new()
        {
            return GetCollection<List<TObject>, TObject>(parser);
        }
        /// <summary>
        /// Create new List,
        /// Create new TObject,
        /// Update Property with TAttribute of TObject,
        /// Add TObject to List,
        /// Return List
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="parser"></param>
        /// <returns>Return Filled List</returns>
        public static List<TObject> GetList<TObject, TAttribute>(this TextFieldParser parser)
            where TObject : new()
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            return parser.GetCollection<List<TObject>, TObject, TAttribute>();
        }
        /// <summary>
        /// Create new List,
        /// Create new ExpandoObject,
        /// Update Property of ExpandoObject,
        /// Execute getValue to get TValue,
        /// Add TValue to List,
        /// Return List
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="parser"></param>
        /// <param name="getValue">Function to get value from dynamic object</param>
        /// <returns>Return Filled List</returns>
        public static List<TValue> GetList<TValue>(this TextFieldParser parser, Func<dynamic, TValue> getValue)
        {
            var toReturn = new List<TValue>();
            parser.AddToCollection(toReturn, getValue);
            return toReturn;
        }
        /// <summary>
        /// Create new List,
        /// Create new ExpandoObject,
        /// Update Property of ExpandoObject,
        /// Add ExpandoObject to List,
        /// Return Hashset
        /// </summary>
        /// <param name="parser"></param>
        /// <returns>Return Filled List</returns>
        public static List<dynamic> GetList(this TextFieldParser parser)
        {
            var toReturn = new List<dynamic>();
            parser.AddToCollection(toReturn);
            return toReturn;
        }
        #endregion


        #region Read Objects, GetKey, Add to Dictionary
        /// <summary>
        /// Create new TObject,
        /// Update property of TObject,
        /// Get TKey from TObject,
        /// Add (TKey,TObject) to Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="parser"></param>
        /// <param name="dictionary">Destination Dictionary</param>
        /// <param name="columnInfos">Column Informations</param>
        /// <param name="getKey">Function to get TKey from TObject</param>
        public static void AddToDictionary<TKey, TObject>(this TextFieldParser parser, IDictionary<TKey, TObject> dictionary, IEnumerable<DelimitedFileColumnInfo> columnInfos, Func<TObject, TKey> getKey)
            where TObject : new()
        {
            while (!parser.EndOfData)
            {
                var item = parser.ReadObject<TObject>(columnInfos);
                var key = getKey(item);
                dictionary.Add(key, item);
            }
        }
        /// <summary>
        /// Create new TObject,
        /// Update property of TObject,
        /// Get TKey from TObject
        /// Add (TKey, TObject) to Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="parser"></param>
        /// <param name="dictionary">Destination Dictionary</param>
        /// <param name="getKey">Function to get TKey from TObject</param>
        public static void AddToDictionary<TKey, TObject>(this TextFieldParser parser, IDictionary<TKey, TObject> dictionary, Func<TObject, TKey> getKey)
            where TObject : new()
        {
            var columnInfos = parser.GetColumnInfos<TObject>();
            parser.AddToDictionary(dictionary, columnInfos, getKey);
        }
        /// <summary>
        /// Create new TObject,
        /// Update property with TAttribute of TObject,
        /// Get TKey from TObject
        /// Add (TKey, TObject) to Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="parser"></param>
        /// <param name="dictionary">Destination Dictionary</param>
        /// <param name="getKey">Function to get TKey from TObject</param>
        public static void AddToDictionary<TKey, TObject, TAttribute>(this TextFieldParser parser, IDictionary<TKey, TObject> dictionary, Func<TObject, TKey> getKey)
            where TObject : new()
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            var columnInfos = parser.GetColumnInfos<TObject, TAttribute>();
            parser.AddToDictionary(dictionary, columnInfos, getKey);
        }
        /// <summary>
        /// Create new ExpandoObject,
        /// Update property of ExpandoObject,
        /// Dethermine whether to Ignore or not,
        /// Get TKey from ExpandoObject,
        /// Get TValue from ExpandoObject,
        /// Add (TKey, TValue) to Dictionary
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="parser"></param>
        /// <param name="dictionary">Destination Dictionary</param>
        /// <param name="getKey">Function to get TKey from ExpandoObject</param>
        /// <param name="getValue">Function to get TValue from ExpandoObject</param>
        /// <param name="isIgnore">Function to determine whether to ignore the item or not</param>
        public static void AddToDictionary<Tkey, TValue>(this TextFieldParser parser, IDictionary<Tkey, TValue> dictionary, Func<dynamic, Tkey> getKey, Func<dynamic, TValue> getValue, Func<IDictionary<Tkey, TValue>, dynamic, bool> isIgnore)
        {
            var columnInfos = parser.GetColumnInfos();

            while (!parser.EndOfData)
            {
                var item = parser.ReadDynamic(columnInfos);
                if (isIgnore(dictionary, item)) continue;
                var key = getKey(item);
                var value = getValue(item);
                dictionary.Add(key, value);
            }
        }
        /// <summary>
        /// Create new ExpandoObject,
        /// Update property of ExpandoObject,
        /// Get TKey from ExpandoObject,
        /// Get TValue from ExpandoObject,
        /// Add (TKey, TValue) to Dictionary
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="parser"></param>
        /// <param name="dictionary">Destination Dictionary</param>
        /// <param name="getKey">Function to get TKey from ExpandoObject</param>
        /// <param name="getValue">Function to get TValue from ExpandoObject</param>
        public static void AddToDictionary<Tkey, TValue>(this TextFieldParser parser, IDictionary<Tkey, TValue> dictionary, Func<dynamic, Tkey> getKey, Func<dynamic, TValue> getValue)
        {
            parser.AddToDictionary(dictionary, getKey, getValue, (dic, d) => false);
        }
        /// <summary>
        /// Create new ExpandoObject,
        /// Update property of ExpandoObject,
        /// Get TKey from ExpandoObject,
        /// Add (TKey, ExpandoObject) to Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="parser"></param>
        /// <param name="dictionary">Destination Dictionary</param>
        /// <param name="getKey">Function to get TKey from ExpandoObject</param
        public static void AddToDictionary<TKey>(this TextFieldParser parser, IDictionary<TKey, dynamic> dictionary, Func<dynamic, TKey> getKey)
        {
            parser.AddToDictionary(dictionary, getKey, d => d);
        }
        #endregion


        #region Read Objects, GetKey, Add to Dictionary, Return Dictionary
        /// <summary>
        /// Create new TObject,
        /// Update property of TObject,
        /// Get TKey from TObject
        /// Add (TKey,TObject) to Dictionary
        /// Return Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="parser"></param>
        /// <param name="getKey">Function to get TKey from ExpandoObject</param
        /// <returns>Return filled Dictionary</returns>
        public static Dictionary<TKey, TObject> GetDictionary<TKey, TObject>(this TextFieldParser parser, Func<TObject, TKey> getKey)
            where TObject : new()
        {
            var toreturn = new Dictionary<TKey, TObject>();
            parser.AddToDictionary(toreturn, getKey);
            return toreturn;
        }
        /// <summary>
        /// Create new TObject,
        /// Update property with TAttribute of TObject,
        /// Get TKey from TObject
        /// Add (TKey,TObject) to Dictionary
        /// Return Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="parser"></param>
        /// <param name="getKey">Function to get TKey from ExpandoObject</param
        /// <returns>Return filled Dictionary</returns>
        public static Dictionary<TKey, TObject> GetDictionary<TKey, TObject, TAttribute>(this TextFieldParser parser, Func<TObject, TKey> getKey)
            where TObject : new()
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            var toreturn = new Dictionary<TKey, TObject>();
            parser.AddToDictionary<TKey, TObject, TAttribute>(toreturn, getKey);
            return toreturn;
        }
        /// <summary>
        /// Create new ExpandoObject,
        /// Update property of ExpandoObject,
        /// Dethermine whether to Ignore or not,
        /// Get TKey from ExpandoObject,
        /// Get TValue from ExpandoObject,
        /// Add (TKey, TValue) to Dictionary
        /// Return Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="parser"></param>
        /// <param name="getKey">Function to get TKey from ExpandoObject</param>
        /// <param name="getValue">Function to get TValue from ExpandoObject</param>
        /// <param name="isIgnore">Function to determine whether to ignore the item or not</param>
        /// <returns>Return filled Dictionary</returns>
        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(this TextFieldParser parser, Func<dynamic, TKey> getKey, Func<dynamic, TValue> getValue, Func<IDictionary<TKey, TValue>, dynamic, bool> isIgnore)
        {
            var toReturn = new Dictionary<TKey, TValue>();
            parser.AddToDictionary(toReturn, getKey, getValue, isIgnore);
            return toReturn;
        }
        /// <summary>
        /// Create new ExpandoObject,
        /// Update property of ExpandoObject,
        /// Get TKey from ExpandoObject,
        /// Get TValue from ExpandoObject,
        /// Add (TKey, TValue) to Dictionary
        /// Return Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="parser"></param>
        /// <param name="getKey">Function to get TKey from ExpandoObject</param>
        /// <param name="getValue">Function to get TValue from ExpandoObject</param>
        /// <returns>Return filled Dictionary</returns>
        public static Dictionary<TKey, TValue> GetDictionary<TKey, TValue>(this TextFieldParser parser, Func<dynamic, TKey> getKey, Func<dynamic, TValue> getValue)
        {
            var toReturn = new Dictionary<TKey, TValue>();
            parser.AddToDictionary(toReturn, getKey, getValue);
            return toReturn;
        }
        /// <summary>
        /// Create new ExpandoObject,
        /// Update property of ExpandoObject,
        /// Get TKey from ExpandoObject,
        /// Add (TKey, ExpandoObject) to Dictionary
        /// Return Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="parser"></param>
        /// <param name="getKey">Function to get TKey from ExpandoObject</param>
        /// <returns>Return filled Dictionary</returns>
        public static Dictionary<TKey, dynamic> GetDictionary<TKey>(this TextFieldParser parser, Func<dynamic, TKey> getKey)
        {
            var toreturn = new Dictionary<TKey, dynamic>();
            parser.AddToDictionary(toreturn, getKey);
            return toreturn;
        }
        #endregion
    }
}
