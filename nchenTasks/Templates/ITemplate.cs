using nchen.Enums;
using nchen.JsonConverters;
using Newtonsoft.Json;

namespace nchen.Templates
{
    [JsonConverter(typeof(ITemplateJsonConverter))]
    public interface ITemplate
    {
        TemplateType Type { get; }
    }
}
