using AdaptiveCards.Enums;

namespace AdaptiveCards.Types
{
    public class TableCell
    {
        public IElement[] Items { get; set; }

        public VerticalAlignment? VerticalContentAlignment { get; set; }
    }
}
