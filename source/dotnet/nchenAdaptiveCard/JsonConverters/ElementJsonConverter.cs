using AdaptiveCards.Enums;
using AdaptiveCards.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace AdaptiveCards.JsonConverters
{
    public class ElementJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(IElement);

        public override bool CanRead => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            if(!jo.TryGetValue(nameof(IElement.Type), StringComparison.InvariantCultureIgnoreCase, out var typeToken))
                throw new NotSupportedException($"Missing {nameof(IElement.Type)} in {jo}.");

            var typeStr = typeToken.Value<string>();
            if (!Enum.TryParse<ElementType>(typeStr, out var type))
                throw new NotSupportedException($"Can't convert {typeStr} to {nameof(ElementType)}.");

            var element = GetNewElement(type);
            serializer.Populate(jo.CreateReader(), element);
            return element;
        }

        public IElement GetNewElement(ElementType type) => type switch
        {
            ElementType.TextBlock => new TextBlock(),
            ElementType.Container => new Container(),
            ElementType.FactSet => new FactSet(),
            ElementType.Table => new Table(),
            ElementType.RichTextBlock => new RichTextBlock(),
            ElementType.TextRun => new TextRun(),
            ElementType.Link => new Link(),
            _ => throw new NotImplementedException($"{type} {nameof(ElementType)} is not implemented to convert to Json."),
        };

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}
