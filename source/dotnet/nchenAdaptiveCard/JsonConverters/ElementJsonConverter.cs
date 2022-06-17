using AdaptiveCards.Enums;
using AdaptiveCards.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

namespace AdaptiveCards.JsonConverters
{
    public class ElementJsonConverter : TypedAbstractJsonConverter<IElement, ElementType>
    {
        protected override string TypePropertyName => nameof(IElement.Type);
        protected override IElement GetObject(ElementType type) => type switch
        {
            ElementType.TextBlock => new TextBlock(),
            ElementType.Container => new Container(),
            ElementType.FactSet => new FactSet(),
            ElementType.Table => new Table(),
            ElementType.RichTextBlock => new RichTextBlock(),
            ElementType.TextRun => new TextRun(),
            ElementType.Link => new Link(),
            ElementType.InputText => new InputText(),
            ElementType.InputNumber => new InputNumber(),
            ElementType.InputDate => new InputDate(),
            ElementType.InputTime => new InputTime(),
            ElementType.InputToggle => new InputToogle(),
            ElementType.InputChoiceSet => new InputChoiceSet(),
            _ => throw new NotImplementedException($"{type} {nameof(ElementType)} is not implemented to convert from Json."),
        };
    }
}
