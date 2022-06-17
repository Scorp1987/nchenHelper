using AdaptiveCards.Enums;
using AdaptiveCards.JsonConverters;
using System.Text.Json.Serialization;

namespace AdaptiveCards.Types
{
    public abstract class AInput
    {
        [JsonConverter(typeof(TextRunArrayJsonConverter))]
        public TextRun[] ErrorMessage { get; set; }
        public string ID { get; set; }
    }
}
