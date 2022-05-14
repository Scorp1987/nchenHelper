using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Newtonsoft.Json.Converters
{
    /// <summary>
    /// Converts an <see cref="KeyValuePair{TKey, TValue}"/> to "{Key}" : {Value} formatted json
    /// </summary>
    public class ImprovedKeyValuePairConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);

        /// <summary>
        /// Reads the JSON representation of the object in "{Key}" : {Value} format
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        /// <exception cref="NotSupportedException">
        /// Type of the value is not <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </exception>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            GetProperties(objectType, out var keyProperty, out var valueProperty);

            var keyConverter = keyProperty.GetTypeConverter();
            var valueConverter = valueProperty.GetTypeConverter();

            // Read from JsonObject
            var jObject = JObject.Load(reader);
            foreach (var prop in jObject)
            {
                var key = keyConverter.ConvertFrom(prop.Key);
                var value = valueConverter.ConvertFrom(prop.Value.ToString());
                return Activator.CreateInstance(objectType, key, value);
            }
            throw new NotSupportedException($"{jObject} can't be converted to {objectType.Name} Type");
        }

        /// <summary>
        /// Write the Json representation of the KeyValuePair to "{Key}" : {Value} format
        /// </summary>
        /// <param name="writer"><see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The object value.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <exception cref="NotSupportedException">
        /// Type of the value is not <see cref="KeyValuePair{TKey, TValue}"/>.
        /// </exception>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var type = value.GetType();
            GetProperties(type, out var keyProperty, out var valueProperty);

            var keyConverter = keyProperty.GetTypeConverter();

            var key = keyProperty.GetValue(value);
            var val = valueProperty.GetValue(value);

            var keyStr = keyConverter.ConvertToString(key);

            // Write Json Object
            writer.WriteStartObject();
            writer.WritePropertyName(keyStr);
            writer.WriteValue(val);
            writer.WriteEndObject();
        }

        private void GetProperties(Type objectType, out PropertyInfo keyProperty, out PropertyInfo valueProperty)
        {
            if (!CanConvert(objectType)) throw new NotSupportedException($"convertion of {objectType.Name} is not supported by {nameof(ImprovedKeyValuePairConverter)}");
            keyProperty = objectType.GetProperty(KEY_NAME);
            valueProperty = objectType.GetProperty(VALUE_NAME);
        }

        private const string KEY_NAME = "Key";
        private const string VALUE_NAME = "Value";
    }
}
