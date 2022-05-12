using AdaptiveCards.Enums;

namespace AdaptiveCards.Types
{
    public class Container : IElement
    {
        public ElementType Type => ElementType.Container;

        public IElement[] Items { get; set; }
    }
}
