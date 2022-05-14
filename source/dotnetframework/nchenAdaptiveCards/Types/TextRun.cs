using AdaptiveCards.Enums;

namespace AdaptiveCards.Types
{
    public class TextRun : IElement
    {
        public ElementType Type => ElementType.TextRun;

        public string Text { get; set; }

        public bool InLineCode { get; set; }

        public FontWeight? Weight { get; set; }

        public bool Italic { get; set; }

        public bool Underline { get; set; }

        public bool Strikethrough { get; set; }

        public bool Spoiler { get; set; }

        public bool Highlight { get; set; }

        public FontSize? Size { get; set; }

        public Color? Color { get; set; }

        public override string ToString() => Text;
    }
}
