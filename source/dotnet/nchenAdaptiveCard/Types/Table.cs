using AdaptiveCards.Enums;

namespace AdaptiveCards.Types
{
    public class Table : IElement
    {
        public ElementType Type => ElementType.Table;

        public bool FirstRowAsHeader { get; set; } = true;

        public TableColumnDefinition[] Columns { get; set; }

        public TableRow[] Rows { get; set; }
    }
}
