using AdaptiveCards.JsonConverters;
using Newtonsoft.Json;

namespace AdaptiveCards.Types
{
    public class AdaptiveCard
    {
        public IElement[] Body { get; set; }
    }
}
