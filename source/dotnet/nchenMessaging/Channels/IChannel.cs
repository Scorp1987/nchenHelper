using nchen.Messaging.JsonConverters;
using nchen.Messaging.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Messaging.Channels
{
    [JsonConverter(typeof(IChannelJsonConverter))]
    public interface IChannel
    {
        ChannelType Type { get; }
        string Name { get; set; }

        Task<object> SendAsync(ITemplate template, object data);
        Task<Dictionary<string, object>> AskInputsAsync(ITemplate template, object data, CancellationToken cancellationToken);
    }
}
