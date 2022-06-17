using System.Attributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace System
{
    public static class TypeExtension
    {
        /// <summary>
        /// Returns all the public properties which have <typeparamref name="TAttribute"/> attribute of the current <see cref="Type"/>
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="type"></param>
        /// <returns>
        /// <para>
        /// An array of <see cref="PropertyInfo"/> objects representing all public properties
        /// , which have the <typeparamref name="TAttribute"/> attribute,
        /// of the current <see cref="Type"/>.
        /// </para>
        /// <para>
        /// An empty array of type <see cref="PropertyInfo"/>,
        /// if the current <see cref="Type"/> does not have public properties.
        /// </para>
        /// </returns>
        public static PropertyInfo[] GetProperties<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            var toReturn = new List<PropertyInfo>();
            foreach(var property in type.GetProperties())
            {
                var attribute = property.GetCustomAttribute<TAttribute>(true);
                if (attribute == default) continue;
                toReturn.Add(property);
            }
            return toReturn.ToArray();
        }


        /// <summary>
        /// Return all the Names of the public properties of the current <see cref="Type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        /// An array of <see cref="string"/> representing all public properties names
        /// of the current <see cref="Type"/>.
        /// <para>
        /// An empty array of <see cref="string"/>
        /// if the current <see cref="Type"/> does not have public properties.
        /// </para>
        /// </returns>
        public static string[] GetNames(this Type type)
        {
            type.CheckClass();
            var query = from property in type.GetProperties()
                        select property.GetName();
            return query.ToArray();
        }
        /// <summary>
        /// Return all the Names of the public properties which have <typeparamref name="TAttribute"/> attribute of the current <see cref="Type"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        /// An array of <see cref="string"/> representing all public properties names
        /// , which have the <typeparamref name="TAttribute"/> attribute,
        /// of the current <see cref="Type"/>.
        /// <para>
        /// An empty array of <see cref="string"/>
        /// if the current <see cref="Type"/> does not have public properties.
        /// </para>
        /// </returns>
        public static string[] GetNames<TAttribute>(this Type type)
            where TAttribute : NamedAttribute
        {
            type.CheckClass();
            var query = from property in type.GetProperties<TAttribute>()
                        select property.GetName<TAttribute>();
            return query.ToArray();
        }


        #region GetDelimitedFileColumnInfos
        public static DelimitedFileColumnInfo[] GetDelimitedFileColumnInfos(this Type type)
        {
            type.CheckClass();
            var query = from property in type.GetProperties()
                        select new DelimitedFileColumnInfo
                        {
                            Name = property.Name,
                            Property = property
                        };

            return query.ToArray();
        }
        public static DelimitedFileColumnInfo[] GetDelimitedFileColumnInfos<TAttribute>(this Type type)
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            type.CheckClass();
            var query = from property in type.GetProperties()
                        let attribute = property.GetCustomAttribute<TAttribute>(true)
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
        #endregion


        /// <summary>
        /// Get the Property with <typeparamref name="TAttribute"/>
        /// </summary>
        /// <typeparam name="TAttribute">
        /// <see cref="NamedAttribute"/> type for the property
        /// </typeparam>
        /// <param name="type"></param>
        /// <param name="name">Name property of the <typeparamref name="TAttribute"/></param>
        /// <returns>
        /// <para><see cref="PropertyInfo"/> that have the</para>
        /// <para>
        /// <see cref="NamedAttribute.Name"/> equals <paramref name="name"/>
        /// if <see cref="NamedAttribute.Name"/> is not <see langword="empty"/> or <see langword="null"/>
        /// </para>
        /// <para>
        /// <see cref="MemberInfo.Name"/> equals <paramref name="name"/>
        /// if <see cref="NamedAttribute.Name"/> is <see langword="empty"/> or <see langword="null"/>
        /// </para>
        /// </returns>
        public static PropertyInfo GetNamedProperty<TAttribute>(this Type type, string name)
            where TAttribute : NamedAttribute
        {
            type.CheckClass();
            var query = from property in type.GetProperties<TAttribute>()
                        let cname = property.GetName<TAttribute>()
                        where cname == name
                        select property;

            var count = query.Count();
            if (count > 1) throw new ArgumentException($"{count} Properties found with {name} name", nameof(name));

            return query.FirstOrDefault();
        }
        public static PropertyInfo GetDelimitedFileProperty<TAttribute>(this Type type, DelimitedFileColumnInfo columnInfo)
            where TAttribute : DelimitedFileColumnInfoAttribute
        {
            type.CheckClass();
            var query = from property in type.GetProperties()
                        let attribute = property.GetCustomAttribute<TAttribute>(true)
                        where attribute != null
                        where (attribute.Index.HasValue && attribute.Index.Value == columnInfo.Index.Value) ||
                            (attribute.Name != null && attribute.Name == columnInfo.Name) ||
                            (string.IsNullOrEmpty(attribute.Name) && property.Name == columnInfo.Name)
                        select property;
            return query.FirstOrDefault();
        }


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
