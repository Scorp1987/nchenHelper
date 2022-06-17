using Newtonsoft.Json.Linq;
using System;

namespace Newtonsoft.Json.Converters
{
    public abstract class TypedAbstractJsonConverter<IObject, IType> : JsonConverter<IObject>
        where IType : struct
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;
        protected virtual string TypePropertyName { get => "Type"; }

        public override IObject ReadJson(JsonReader reader, Type objectType, IObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            if (!jo.TryGetValue(TypePropertyName, StringComparison.InvariantCultureIgnoreCase, out var typeToken))
                throw new NotSupportedException($"Missing {TypePropertyName} in {jo}.");

            var typeStr = typeToken.Value<string>();
            var type = typeStr.ToEnum<IType>();

            var obj = GetObject(type);
            serializer.Populate(jo.CreateReader(), obj);
            return obj;
        }
        public override void WriteJson(JsonWriter writer, IObject value, JsonSerializer serializer) => throw new NotImplementedException();

        protected abstract IObject GetObject(IType type);
    }
}
