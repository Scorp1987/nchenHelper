using AdaptiveCards.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace AdaptiveCards.JsonConverters
{
    public class TextRunArrayJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string) || objectType == typeof(TextRun);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var toReturn = new List<TextRun>();

            var token = JToken.Load(reader);
            if (token is JArray)
            {
                foreach (var item in token)
                    if (item is JObject)
                        toReturn.Add(JsonConvert.DeserializeObject<TextRun>(item.ToString()));
                    else if (item is JValue)
                        toReturn.Add(new TextRun { Text = item.Value<string>() });
            }
            else if (token is JObject)
                toReturn.Add(JsonConvert.DeserializeObject<TextRun>(token.ToString()));
            else if (token is JValue)
                toReturn.Add(new TextRun { Text = token.Value<string>() });

            return toReturn.ToArray();
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new NotImplementedException();
    }
}
