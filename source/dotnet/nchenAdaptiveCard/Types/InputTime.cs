using AdaptiveCards.Enums;
using AdaptiveCards.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AdaptiveCards.Types
{
    public class InputTime : AInput, IElement, IInput
    {
        [JsonConverter(typeof(TextRunArrayJsonConverter))]
        public TextRun[] PlaceHolder { get; set; }
        public DateTime? Min { get; set; }
        public DateTime? Max { get; set; }
        public ElementType Type => ElementType.InputTime;
        public TextRun[] Title => PlaceHolder;
    }
}
