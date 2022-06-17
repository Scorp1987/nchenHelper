using nchen.Messaging.Channels;
using nchen.Messaging.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.Messaging.Templates
{
    [JsonConverter(typeof(ITemplateJsonConverter))]
    public interface ITemplate
    {
        ChannelType Type { get; }
    }
}
