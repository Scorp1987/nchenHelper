using AdaptiveCards.Enums;

namespace AdaptiveCards.Types
{
    public class FactSet : IElement
    {
        public ElementType Type => ElementType.FactSet;

        public Fact[] Facts { get; set; }

        public bool Seperator { get; set; } = true;
    }
}
