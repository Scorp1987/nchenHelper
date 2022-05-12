using AdaptiveCards.Enums;
using AdaptiveCards.JsonConverters;
using Newtonsoft.Json;

namespace AdaptiveCards.Types
{
    [JsonConverter(typeof(ElementJsonConverter))]
    public interface IElement
    {
        ElementType Type { get; }
    }
}
