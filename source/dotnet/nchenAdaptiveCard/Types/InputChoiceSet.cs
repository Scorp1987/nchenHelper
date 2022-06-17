using AdaptiveCards.Enums;
using AdaptiveCards.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AdaptiveCards.Types
{
    public class InputChoiceSet : AInput, IElement, IInput
    {
        [JsonConverter(typeof(TextRunArrayJsonConverter))]
        public TextRun[] PlaceHolder { get; set; }
        public InputChoice[] Choices { get; set; }
        public int MaxWidth { get; set; } = 1;
        public ElementType Type => ElementType.InputChoiceSet;
        public TextRun[] Title => PlaceHolder;
    }
}
