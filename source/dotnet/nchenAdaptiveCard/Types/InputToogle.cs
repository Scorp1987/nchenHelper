using AdaptiveCards.Enums;
using AdaptiveCards.JsonConverters;
using System.Text.Json.Serialization;

namespace AdaptiveCards.Types
{
    public class InputToogle : IElement, IInput
    {
        public ElementType Type => ElementType.InputToggle;
        public string ID { get; set; }
        public string ValueOn { get; set; }
        public string ValueOff { get; set; }


        [JsonConverter(typeof(TextRunArrayJsonConverter))]
        public TextRun[] Title { get; set; }


        [JsonConverter(typeof(TextRunArrayJsonConverter))]
        public TextRun[] ErrorMessage { get; set; }
    }
}
