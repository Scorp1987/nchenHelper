using nchen.Messaging.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.Messaging.JsonConverters
{
    public class ITemplateArrayJsonConverter : JsonConverter<ITemplate[]>
    {
        public override ITemplate[] ReadJson(JsonReader reader, Type objectType, ITemplate[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var toReturn = new List<ITemplate>();
            var token = JToken.Load(reader);
            if(token is JArray)
            {
                foreach (var item in token)
                    toReturn.Add(JsonConvert.DeserializeObject<ITemplate>(item.ToString()));
            }
            else if(token is JObject)
                toReturn.Add(JsonConvert.DeserializeObject<ITemplate>(token.ToString()));

            return toReturn.ToArray();
        }

        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, ITemplate[] value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
