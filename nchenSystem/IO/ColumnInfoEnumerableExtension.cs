using System.Collections.Generic;
using System.IO.Types;
using System.Linq;
using System.Reflection;

namespace System.IO
{
    public static class ColumnInfoEnumerableExtension
    {
        public static DelimitedFileColumnInfo GetColumnInfo(this IEnumerable<DelimitedFileColumnInfo> collection, string name)
        {
            return (from columnInfo in collection
                    where columnInfo.Name == name
                    select columnInfo).FirstOrDefault();
        }

        public static DelimitedFileColumnInfo GetColumnInfo(this IEnumerable<DelimitedFileColumnInfo> collection, int index)
        {
            return (from columnInfo in collection
                    where columnInfo.Index == index
                    select columnInfo).FirstOrDefault();
        }

        public static DelimitedFileColumnInfo GetColumnInfo(this IEnumerable<DelimitedFileColumnInfo> collection, PropertyInfo property)
        {
            return (from columnInfo in collection
                    where columnInfo.Property == property
                    select columnInfo).FirstOrDefault();
        }

        public static int GetMaxIndex(this IEnumerable<DelimitedFileColumnInfo> collection) => collection.Max(r => r.Index);

        public static IEnumerable<PropertyInfo> GetPropertyInfos(this IEnumerable<DelimitedFileColumnInfo> collection)
        {
            return from columnInfo in collection
                   orderby columnInfo.Index
                   select columnInfo.Property;
        }
    }
}
