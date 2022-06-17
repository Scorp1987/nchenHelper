using System.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace System.IO
{
    public static class StreamWriterExtension
    {
        #region Convert to string that is ready to write to delimited stream
        /// <summary>
        /// Get string that is ready to write to delimited stream.
        /// Put quote if necessary.
        /// </summary>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="valueStr">string value</param>
        /// <returns>string value that is ready to be written to delimited stream</returns>
        private static string GetString(string delimiter, string valueStr)
        {
            if (string.IsNullOrEmpty(valueStr)) return "";

            var containQuote = valueStr.Contains("\"");
            var needQuote = containQuote || valueStr.Contains("\r") || valueStr.Contains("\n") || valueStr.Contains(delimiter);
            if (containQuote) valueStr = valueStr.Replace("\"", "\"\"");

            if (needQuote) return $"\"{valueStr}\"";
            else return valueStr;
        }
        /// <summary>
        /// Get string that is ready to write to delimited stream.
        /// </summary>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="value">Field value to write</param>
        /// <param name="converter">TypeConverter to convert value to string</param>
        /// <returns>string value that is ready to be written to delimited stream</returns>
        private static string GetString(string delimiter, object value, TypeConverter converter)
        {
            if (converter is NullableConverter nc && value != null)
                converter = nc.UnderlyingTypeConverter;
            if (converter.GetType() == typeof(DateTimeConverter))
                converter = new ISODateTimeConverter();

            var valueStr = converter.ConvertToString(value);
            return GetString(delimiter, valueStr);
        }
        /// <summary>
        /// Get string that is ready to write to delimited stream.
        /// </summary>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="value">Field value to write</param>
        /// <returns>string value that is ready to be written to delimited stream</returns>
        private static string GetString(string delimiter, object value)
        {
            if (value == null) return "";
            var converter = TypeDescriptor.GetConverter(value.GetType());
            return GetString(delimiter, value, converter);
        }
        /// <summary>
        /// Get string that is ready to write to delimited stream.
        /// </summary>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="values">list of values to write</param>
        /// <returns></returns>
        private static string GetString(string delimiter, IEnumerable values)
        {
            string toReturn = "";
            foreach (var value in values)
            {
                string valueStr = GetString(delimiter, value);
                toReturn += $"{delimiter}{valueStr}";
            }
            return toReturn[delimiter.Length..];
        }
        #endregion


        #region Get Column Names
        //private static IEnumerable<string> GetColumnNames<TObject>()
        //{
        //    var query = from property in typeof(TObject).GetProperties()
        //                let attribute = property.GetCustomAttributes<DelimitedFileColumnNameAttribute>().FirstOrDefault()
        //                select attribute?.Name ?? property.Name;
        //    return query;
        //    //return query.ToArray();
        //}
        //private static IEnumerable<string> GetColumnNames<TObject, TAttribute>()
        //    where TAttribute : DelimitedFileColumnNameAttribute
        //{
        //    var query = from property in typeof(TObject).GetProperties()
        //                let attribute = property.GetCustomAttribute<TAttribute>()
        //                where attribute != null
        //                select attribute.Name ?? property.Name;
        //    return query;
        //    //return query.ToArray();
        //}
        private static IEnumerable<string> GetColumnNames(IEnumerable<dynamic> collection)
        {
            var query = from item in collection
                        from name in (item is IDictionary<string, object> dic) ? dic.Keys : ((Type)item.GetType()).GetProperties().Select(p => p.Name)
                        group name by name into gp1
                        select gp1.Key;
            return query;
            //return query.ToArray();
        }
        #endregion


        #region Write a row values to delimited stream
        /// <summary>
        /// Write list of values to delimited stream
        /// </summary>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="values">Collection of values</param>
        public static void WriteFields(this StreamWriter writer, string delimiter, IEnumerable values)
        {
            string toWrite = GetString(delimiter, values);
            writer.WriteLine(toWrite);
        }
        /// <summary>
        /// Write list of values to delimited stream
        /// </summary>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="values">Collection of values</param>
        /// <returns></returns>
        public static Task WriteFieldsAsync(this StreamWriter writer, string delimiter, IEnumerable values)
        {
            string toWrite = GetString(delimiter, values);
            return writer.WriteLineAsync(toWrite);
        }
        #endregion


        #region Get column names and write a row to delimited stream
        /// <summary>
        /// Write names of column to delimited stream
        /// </summary>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">>Delimiter to seperate values</param>
        /// <param name="columnInfos">Information of the column</param>
        public static void WriteColumnNames(this StreamWriter writer, string delimiter, IEnumerable<DelimitedFileColumnInfo> columnInfos)
        {
            var query = from columnInfo in columnInfos
                        select columnInfo.Name;
            writer.WriteFields(delimiter, query);
        }
        /// <summary>
        /// Write names of column to delimited stream
        /// </summary>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">>Delimiter to seperate values</param>
        /// <param name="columnInfos">Information of the column</param>
        /// <returns></returns>
        public static async Task WriteColumnNamesAsync(this StreamWriter writer, string delimiter, IEnumerable<DelimitedFileColumnInfo> columnInfos)
        {
            var query = from columnInfo in columnInfos
                        select columnInfo.Name;
            await writer.WriteFieldsAsync(delimiter, query);
        }
        #endregion


        #region Write a row of values from object property to delimited stream
        /// <summary>
        /// Write values of TObject's properties to delimited stream
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="obj">Source object</param>
        /// <param name="properties">List of properties in the order to write to delimited stream</param>
        public static void WriteFields<TObject>(this StreamWriter writer, string delimiter, TObject obj, IEnumerable<PropertyInfo> properties)
            where TObject : class
        {
            if (properties.Count() < 1) return;
            var toWrite = "";
            foreach (var property in properties)
            {
                string valueStr = "";
                if (property != null)
                {
                    var converter = property.GetTypeConverter();
                    var value = property.GetValue(obj);
                    valueStr = GetString(delimiter, value, converter);
                }
                toWrite += $"{delimiter}{valueStr}";
            }
            toWrite = toWrite[delimiter.Length..];
            writer.WriteLine(toWrite);
        }
        /// <summary>
        /// Write values of TObject's properties to delimited stream
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="obj">Source object</param>
        /// <param name="properties">List of properties in the order to write to delimited stream</param>
        /// <returns></returns>
        public static async Task WriteFieldsAsync<TObject>(this StreamWriter writer, string delimiter, TObject obj, IEnumerable<PropertyInfo> properties)
            where TObject : class
        {
            if (properties.Count() < 1) return;
            var toWrite = "";
            foreach (var property in properties)
            {
                string valueStr = "";
                if (property != null)
                {
                    var converter = property.GetTypeConverter();
                    var value = property.GetValue(obj);
                    valueStr = GetString(delimiter, value, converter);
                }
                toWrite += $"{delimiter}{valueStr}";
            }
            toWrite = toWrite[delimiter.Length..];
            await writer.WriteLineAsync(toWrite);
        }

        /// <summary>
        /// Write values of TObject's properties to delimited stream
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="obj">Source object</param>
        /// <param name="columnInfos">Column Informations</param>
        public static void WriteFields<TObject>(this StreamWriter writer, string delimiter, TObject obj, IEnumerable<DelimitedFileColumnInfo> columnInfos)
            where TObject : class
        {
            var properties = columnInfos.GetPropertyInfos();
            writer.WriteFields(delimiter, obj, properties);
        }
        /// <summary>
        /// Write values of TObject's properties to delimited stream
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="obj">Source object</param>
        /// <param name="columnInfos">Column Informations</param>
        /// <returns></returns>
        public static Task WriteFieldsAsync<TObject>(this StreamWriter writer, string delimiter, TObject obj, IEnumerable<DelimitedFileColumnInfo> columnInfos)
            where TObject : class
        {
            var properties = columnInfos.GetPropertyInfos();
            return writer.WriteFieldsAsync(delimiter, obj, properties);
        }

        /// <summary>
        /// Write values of dynamic object properties to delimited stream
        /// </summary>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="obj">Source object</param>
        /// <param name="headers">List of Column Names in order</param>
        public static void WriteFields(this StreamWriter writer, string delimiter, dynamic obj, IEnumerable<string> columnNames)
        {
            if (obj is IDictionary<string, object> dic)
            {
                var values = from cn in columnNames
                             join item in dic on cn equals item.Key into gp1
                             from item in gp1.DefaultIfEmpty(new KeyValuePair<string, object>(cn, null))
                             select item.Value;
                writer.WriteFields(delimiter, (IEnumerable)values);
            }
            else
            {
                var values = from cn in columnNames
                             join property in ((Type)obj.GetType()).GetProperties() on cn equals property.Name into gp1
                             from property in gp1.DefaultIfEmpty()
                             select property?.GetValue(obj);
                writer.WriteFields(delimiter, (IEnumerable)values);
            }
        }
        /// <summary>
        /// Write values of dynamic object properties to delimited stream
        /// </summary>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="obj">Source object</param>
        /// <param name="headers">List of Column Names in order</param>
        /// <returns></returns>
        public static Task WriteFieldsAsync(this StreamWriter writer, string delimiter, dynamic obj, IEnumerable<string> columnNames)
        {
            if (obj is IDictionary<string, object> dic)
            {
                var values = from cn in columnNames
                             join item in dic on cn equals item.Key into gp1
                             from item in gp1.DefaultIfEmpty(new KeyValuePair<string, object>(cn, null))
                             select item.Value;
                return writer.WriteFieldsAsync(delimiter, (IEnumerable)values);
            }
            else
            {
                var values = from cn in columnNames
                             join property in ((Type)obj.GetType()).GetProperties() on cn equals property.Name into gp1
                             from property in gp1.DefaultIfEmpty()
                             select property?.GetValue(obj);
                return writer.WriteFieldsAsync(delimiter, (IEnumerable)values);
            }
        }

        #endregion


        #region Write multiple rows of values from collection of object to delimited stream
        /// <summary>
        /// Write collection of TObject's properties' values to delimited stream
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="collection">Source Collection of object</param>
        /// <param name="writeHeader">To write header row, set to true. Otherwise set to false</param>
        public static void WriteCollection<TObject>(this StreamWriter writer, string delimiter, IEnumerable<TObject> collection, bool writeHeader = true)
            where TObject : class
        {
            var columnInfos = typeof(TObject).GetDelimitedFileColumnInfos();
            if (writeHeader)
                writer.WriteColumnNames(delimiter, columnInfos);
            //writer.WriteFields<TObject>(delimiter);

            foreach (var item in collection)
                writer.WriteFields(delimiter, item, columnInfos);
        }
        /// <summary>
        /// Write collection of TObject's properties' values to delimited stream
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="collection">Source Collection of object</param>
        /// <param name="writeHeader">To write header row, set to true. Otherwise set to false</param>
        /// <returns></returns>
        public static async Task WriteCollectionAsync<TObject>(this StreamWriter writer, string delimiter, IEnumerable<TObject> collection, bool writeHeader = true)
            where TObject : class
        {
            var columnInfos = typeof(TObject).GetDelimitedFileColumnInfos();
            if (writeHeader)
                await writer.WriteColumnNamesAsync(delimiter, columnInfos);

            foreach (var item in collection)
                await writer.WriteFieldsAsync(delimiter, item, columnInfos);
        }

        /// <summary>
        /// Write collection of TObject's properties' values with TAttribute to delimited stream
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="collection">Source Collection of object</param>
        /// <param name="writeHeader">To write header row, set to true. Otherwise set to false</param>
        public static void WriteCollection<TObject, TAttribute>(this StreamWriter writer, string delimiter, IEnumerable<TObject> collection, bool writeHeader = true)
            where TObject : class
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            var columnInfos = typeof(TObject).GetDelimitedFileColumnInfos<TAttribute>();
            if (writeHeader)
                writer.WriteColumnNames(delimiter, columnInfos);
            //writer.WriteFields(",", columnNames);

            foreach (var item in collection)
                writer.WriteFields(delimiter, item, columnInfos);
        }
        /// <summary>
        /// Write collection of TObject's properties' values with TAttribute to delimited stream
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="collection">Source Collection of object</param>
        /// <param name="writeHeader">To write header row, set to true. Otherwise set to false</param>
        /// <returns></returns>
        public static async Task WriteCollectionAsync<TObject, TAttribute>(this StreamWriter writer, string delimiter, IEnumerable<TObject> collection, bool writeHeader = true)
            where TObject : class
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            var columnInfos = typeof(TObject).GetDelimitedFileColumnInfos<TAttribute>();
            if (writeHeader)
                await writer.WriteColumnNamesAsync(delimiter, columnInfos);
            //await writer.WriteFieldsAsync<TObject, TAttribute>(delimiter);

            foreach (var item in collection)
                await writer.WriteFieldsAsync(delimiter, item, columnInfos);
        }

        /// <summary>
        /// Write collection of dynamic object's properties' values to delimited stream
        /// </summary>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="collection">Source Collection of object</param>
        /// <param name="columnNames">List of Column Names</param>
        /// <param name="writeHeader">To write header row, set to true. Otherwise set to false</param>
        public static void WriteCollection(this StreamWriter writer, string delimiter, IEnumerable<dynamic> collection, IEnumerable<string> columnNames, bool writerHeader = true)
        {
            if (writerHeader)
                writer.WriteFields(delimiter, columnNames);

            foreach (var item in collection)
                WriteFields(writer, delimiter, item, columnNames);
        }
        /// <summary>
        /// Write collection of dynamic object's properties' values to delimited stream
        /// </summary>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="collection">Source Collection of object</param>
        /// <param name="columnNames">List of Column Names</param>
        /// <param name="writeHeader">To write header row, set to true. Otherwise set to false</param>
        /// <returns></returns>
        public static async Task WriteCollectionAsync(this StreamWriter writer, string delimiter, IEnumerable<dynamic> collection, IEnumerable<string> columnNames, bool writerHeader = true)
        {
            if (writerHeader)
                await writer.WriteFieldsAsync(delimiter, columnNames);

            foreach (var item in collection)
                await WriteFieldsAsync(writer, delimiter, item, columnNames);
        }

        /// <summary>
        /// Write collection of dynamic object's properties' values to delimited stream
        /// </summary>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="collection">Source Collection of object</param>
        /// <param name="writeHeader">To write header row, set to true. Otherwise set to false</param>
        public static void WriteCollection(this StreamWriter writer, string delimiter, IEnumerable<dynamic> collection, bool writeHeader = true)
        {
            var columnNames = GetColumnNames(collection);
            writer.WriteCollection(delimiter, collection, columnNames, writeHeader);
        }
        /// <summary>
        /// Write collection of dynamic object's properties' values to delimited stream
        /// </summary>
        /// <param name="writer">Destination Stream Writer</param>
        /// <param name="delimiter">Delimiter to seperate values</param>
        /// <param name="collection">Source Collection of object</param>
        /// <param name="writeHeader">To write header row, set to true. Otherwise set to false</param>
        /// <returns></returns>
        public static Task WriteCollectionAsync(this StreamWriter writer, string delimiter, IEnumerable<dynamic> collection, bool writeHeader = true)
        {
            var columnNames = GetColumnNames(collection);
            return writer.WriteCollectionAsync(delimiter, collection, columnNames, writeHeader);
        }
        #endregion
    }
}
