using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Newtonsoft.Json.Converters
{
    /// <summary>
    /// Converts each item in generic array object with <typeparamref name="TItemConverter"/> converter
    /// </summary>
    /// <typeparam name="TItemConverter">Converter to be used to convert for each item in generic array object</typeparam>
    public class GenericArrayConverter<TItemConverter> : JsonConverter
        where TItemConverter : JsonConverter, new()
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType == null) throw new ArgumentNullException(nameof(objectType));

            var interfaces = objectType.GetInterfaces();
            foreach (var @interface in interfaces)
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICollection<>))
                    return true;
            return false;
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        /// <exception cref="NotSupportedException">
        /// Type of the value is not <see cref="ICollection{T}"/>.
        /// </exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!CanConvert(objectType)) throw GetNotSupportedException(objectType);

            var itemConverter = GetItemConverter();
            var itemType = objectType.GetGenericArguments()[0];

            dynamic list = Activator.CreateInstance(objectType);
            var jArray = JToken.Load(reader);
            foreach (var jItem in jArray)
            {
                var itemReader = new JTokenReader(jItem);
                dynamic item = itemConverter.ReadJson(itemReader, itemType, jItem, serializer);
                list.Add(item);
            }
            return list;
        }


        /// <summary>
        /// Write the Json representation of the object
        /// </summary>
        /// <param name="writer"><see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The object value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="NotSupportedException">
        /// Type of the value is not <see cref="IEnumerable"/>.
        /// </exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();
            if (!(value is IEnumerable list)) throw GetNotSupportedException(type);

            var itemConverter = GetItemConverter();
            writer.WriteStartArray();
            foreach (var item in list)
                itemConverter.WriteJson(writer, item, serializer);
            writer.WriteEndArray();
        }

        private static TItemConverter GetItemConverter() => new TItemConverter();
        private static NotSupportedException GetNotSupportedException(Type type)
            => new NotSupportedException($"convertion of {type.Name} is not supported by {nameof(GenericArrayConverter<TItemConverter>)}");
    }
}
