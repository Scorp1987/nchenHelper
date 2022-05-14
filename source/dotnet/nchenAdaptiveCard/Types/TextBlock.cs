using AdaptiveCards.Enums;

namespace AdaptiveCards.Types
{
    public class TextBlock : IElement
    {
        public ElementType Type => ElementType.TextBlock;

        public string Text { get; set; }

        public Color? Color { get; set; }

        public FontSize? Size { get; set; }

        public HorizontalAlignment? HorizontalAlignment { get; set; }

        public FontWeight? Weight { get; set; }

        public int MaxLines { get; set; }

        public bool Wrap { get; set; }

        public override string ToString() => Text;
    }
}
