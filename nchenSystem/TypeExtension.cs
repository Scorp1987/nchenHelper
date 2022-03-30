using System.Collections.Generic;
using System.Data.Attributes;
using System.Data.Exceptions;
using System.IO.Attributes;
using System.IO.Types;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class TypeExtension
    {
        #region GetDataTableColumnName(s)
        /// <summary>
        /// Get columns' name for public properties of <paramref name="type"/>
        /// with <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">
        /// <see cref="DataColumnInfoAttribute"/> type for the properties
        /// </typeparam>
        /// <param name="type"></param>
        /// <returns>
        /// <see cref="MemberInfo.Name"/> if <see cref="DataColumnInfoAttribute.Name"/> is <see langword="null"/>,
        /// otherwise <see cref="DataColumnInfoAttribute.Name"/>
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// <paramref name="type"/> is not <see langword="class"/>
        /// </exception>
        public static IEnumerable<string> GetDataTableColumnNames<TAttribute>(this Type type)
            where TAttribute : DataColumnInfoAttribute
        {
            type.CheckClass();
            return from property in type.GetProperties()
                   let attributes = property.GetCustomAttributes<TAttribute>()
                   where attributes.Count() > 0
                   select property.GetDataTableColumnName<TAttribute>();
        }

        /// <summary>
        /// Get the name of the column for property with <typeparamref name="TAttribute"/>
        /// </summary>
        /// <typeparam name="TAttribute">
        /// <see cref="DataColumnInfoAttribute"/> type for the property
        /// </typeparam>
        /// <param name="type"></param>
        /// <param name="propertyName">name of the property of <paramref name="type"/></param>
        /// <returns>
        /// <paramref name="propertyName"/> if <see cref="DataColumnInfoAttribute.Name"/> is <see langword="null"/>,
        /// otherwise <see cref="DataColumnInfoAttribute.Name"/>
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// <paramref name="type"/> is not <see langword="class"/>
        /// </exception>
        /// <exception cref="PropertyInfoNotFoundException">
        /// Can't find property with <paramref name="propertyName"/>
        public static string GetDataTableColumnName<TAttribute>(this Type type, string propertyName)
            where TAttribute : DataColumnInfoAttribute
        {
            type.CheckClass();
            var property = type.GetProperty(propertyName);
            if (property == null) throw new PropertyInfoNotFoundException(propertyName);

            return property.GetDataTableColumnName<TAttribute>();
        }

        /// <summary>
        /// Get the Property with <typeparamref name="TAttribute"/>
        /// </summary>
        /// <typeparam name="TAttribute">
        /// <see cref="DataColumnInfoAttribute"/> type for the property
        /// </typeparam>
        /// <param name="type"></param>
        /// <param name="columnName"></param>
        /// <returns>
        /// <paramref name="propertyName"/> if <see cref="DataColumnInfoAttribute.Name"/> is <see langword="null"/>,
        /// otherwise <see cref="DataColumnInfoAttribute.Name"/>
        /// </returns>
        public static PropertyInfo GetDataTableProperty<TAttribute>(this Type type, string columnName)
            where TAttribute : DataColumnInfoAttribute
        {
            type.CheckClass();
            var query = from property in type.GetProperties()
                        let cname = property.GetDataTableColumnName<TAttribute>()
                        where cname == columnName
                        select property;
            return query.FirstOrDefault();
        }
        #endregion


        #region GetDelimitedFileColumnInfos
        public static DelimitedFileColumnInfo[] GetDelimitedFileColumnInfos(this Type type)
        {
            type.CheckClass();
            var query = from property in type.GetProperties()
                            //let attribute = property.GetCustomAttributes<DelimitedFileColumnIndexAttribute>().FirstOrDefault()
                            //where attribute != null
                        select new DelimitedFileColumnInfo
                        {
                            Name = property.Name,
                            //Index = attribute.Index,
                            Property = property
                        };

            return query.ToArray();
        }

        public static DelimitedFileColumnInfo[] GetDelimitedFileColumnInfos<TAttribute>(this Type type)
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            type.CheckClass();
            //var properties = from property in type.GetProperties()
            //                 let attribute = property.GetCustomAttributes<TAttribute>().FirstOrDefault()
            //                 where attribute != null
            //                 orderby attribute.Index
            //                 select new { property, attribute };

            //var maxIndex = (from info in properties
            //                select info.attribute?.Index).Max();
            //if (maxIndex == null) return new DelimitedFileColumnInfo[0];

            //var toReturn = new DelimitedFileColumnInfo[maxIndex.Value + 1];
            //var emulator = properties.GetEnumerator();
            //for (int i = 0; i <= maxIndex; i++)
            //{
            //    if (emulator.MoveNext())
            //    {
            //        toReturn[i] = new DelimitedFileColumnInfo
            //        {
            //            Name = string.IsNullOrEmpty(emulator.Current.attribute.Name) ? emulator.Current.property.Name : emulator.Current.attribute.Name,
            //            Index = i,
            //            Property = emulator.Current.property
            //        };
            //    }
            //    else
            //    {
            //        toReturn[i] = new DelimitedFileColumnInfo
            //        {
            //            Name = "",
            //            Index = i,
            //            Property = null
            //        };
            //    }
            //}
            //return toReturn;

            var query = from property in type.GetProperties()
                        let attribute = property.GetCustomAttributes<TAttribute>().FirstOrDefault()
                        where attribute != null
                        orderby attribute.Index
                        select new DelimitedFileColumnInfo
                        {
                            Name = string.IsNullOrEmpty(attribute.Name) ? property.Name : attribute.Name,
                            Index = attribute.Index,
                            Property = property
                        };
            return query.ToArray();
        }

        public static PropertyInfo GetDelimitedFileProperty<TAttribute>(this Type type, DelimitedFileColumnInfo columnInfo)
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            type.CheckClass();
            var query = from property in type.GetProperties()
                        let attribute = property.GetAttribute<TAttribute>()
                        where attribute != null
                        where (attribute.Index.HasValue && attribute.Index.Value == columnInfo.Index.Value) ||
                            (attribute.Name != null && attribute.Name == columnInfo.Name) ||
                            (string.IsNullOrEmpty(attribute.Name) && property.Name == columnInfo.Name)
                        select property;
            return query.FirstOrDefault();
        }

        //public static DelimitedFileColumnInfo[] GetDelimitedFileColumnInfosByIndex<TIndexAttribute>(this Type type)
        //    where TIndexAttribute : DelimitedFileColumnIndexAttribute
        //{
        //    type.CheckClass();
        //    var query = from property in type.GetProperties()
        //                let attribute = property.GetCustomAttributes<TIndexAttribute>().FirstOrDefault()
        //                where attribute != null
        //                select new DelimitedFileColumnInfo
        //                {
        //                    Name = property.Name,
        //                    Index = attribute.Index,
        //                    Property = property
        //                };

        //    return query.ToArray();
        //}

        //public static DelimitedFileColumnInfo[] GetDelimitedFileColumnInfosByName<TNameAttribute>(this Type type)
        //    where TNameAttribute : DelimitedFileColumnNameAttribute
        //{
        //    type.CheckClass();
        //    var query = from property in type.GetProperties()
        //                let attribute = property.GetCustomAttributes<TNameAttribute>().FirstOrDefault()
        //                where attribute != null
        //                select new DelimitedFileColumnInfo
        //                {
        //                    Name = property.Name,
        //                    Property = property
        //                };
        //    return query.ToArray();
        //}

        //public static DelimitedFileColumnInfo[] GetDelimitedFileColumnInfos<TIndexAttribute, TNameAttribute>(this Type type)
        //    where TIndexAttribute : DelimitedFileColumnIndexAttribute
        //    where TNameAttribute : DelimitedFileColumnNameAttribute
        //{
        //    type.CheckClass();
        //    var query = from property in type.GetProperties()
        //                let indexAttribute = property.GetCustomAttributes<TIndexAttribute>().FirstOrDefault()
        //                let nameAttribute = property.GetCustomAttributes<TNameAttribute>().FirstOrDefault()
        //                where indexAttribute != null && nameAttribute != null
        //                select new DelimitedFileColumnInfo
        //                {
        //                    Name = nameAttribute.Name ?? property.Name,
        //                    Index = indexAttribute.Index,
        //                    Property = property
        //                };

        //    return query.ToArray();
        //}
        #endregion


        /// <summary>
        /// Check Type and throw exception if needed
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="NotSupportedException">
        /// <paramref name="type"/> is not <see langword="class"/>
        /// </exception>
        private static void CheckClass(this Type type)
        {
            if (!type.IsClass) throw new NotSupportedException($"{type.FullName} must be a class.");
        }
    }
}
