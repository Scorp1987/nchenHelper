using nchen.Enums;
using nchen.JsonConverters;
using nchen.Templates;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace nchen.Channels
{
    [JsonConverter(typeof(IChannelJsonConverter))]
    public interface IChannel
    {
        ChannelType Type { get; }
        string Name { get; set; }

        Task<string> SendAsync(ITemplate template, object data);
    }
}
