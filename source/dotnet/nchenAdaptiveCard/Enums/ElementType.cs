using System.ComponentModel;
using System.Runtime.Serialization;

namespace AdaptiveCards.Enums
{
    public enum ElementType
    {
        TextBlock,
        RichTextBlock,
        TextRun,
        Link,
        FactSet,
        Container,
        Table,
        ColumnSet,
        [EnumMember(Value = "Input.Text")]
        InputText,
        [EnumMember(Value = "Input.Number")]
        InputNumber,
        [EnumMember(Value = "Input.Date")]
        InputDate,
        [EnumMember(Value = "Input.Time")]
        InputTime,
        [EnumMember(Value = "Input.Toggle")]
        InputToggle,
        [EnumMember(Value = "Input.ChoiceSet")]
        InputChoiceSet
    }
}
