using AdaptiveCards.Enums;
using AdaptiveCards.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AdaptiveCards.Types
{
    public class InputNumber : AInput, IElement, IInput
    {
        [JsonConverter(typeof(TextRunArrayJsonConverter))]
        public TextRun[] PlaceHolder { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public ElementType Type => ElementType.InputNumber;
        public TextRun[] Title => PlaceHolder;
    }
}
