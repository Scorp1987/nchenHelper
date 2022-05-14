using AdaptiveCards.JsonConverters;
using Newtonsoft.Json;

namespace AdaptiveCards.Types
{
    public class Fact
    {
        [JsonConverter(typeof(TextRunArrayJsonConverter))]
        public TextRun[] Title { get; set; }

        [JsonConverter(typeof(TextRunArrayJsonConverter))]
        public TextRun[] Value { get; set; }

        public override string ToString()
        {
            var title = "";
            var value = "";
            foreach (var item in Title) title += item.Text;
            foreach (var item in Value) value += item.Text;
            return $"{title} : {value}";
        }
    }
}
