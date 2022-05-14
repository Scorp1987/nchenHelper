using AdaptiveCards.Enums;
using AdaptiveCards.JsonConverters;
using Newtonsoft.Json;

namespace AdaptiveCards.Types
{
    public class RichTextBlock : IElement
    {
        public ElementType Type => ElementType.RichTextBlock;

        public HorizontalAlignment? HorizontalAlignment { get; set; }

        [JsonConverter(typeof(TextRunArrayJsonConverter))]
        public TextRun[] Inlines { get; set; }
    }
}
