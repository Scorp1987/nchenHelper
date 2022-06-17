using AdaptiveCards.Enums;
using AdaptiveCards.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AdaptiveCards.Types
{
    public class InputText : AInput, IElement, IInput
    {
        [JsonConverter(typeof(TextRunArrayJsonConverter))]
        public TextRun[] PlaceHolder { get; set; }
        public int? MaxLength { get; set; }
        public string Regex { get; set; }
        public ElementType Type => ElementType.InputText;
        public TextRun[] Title => PlaceHolder;
    }
}
