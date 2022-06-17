using nchen.Messaging.Channels;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.Messaging.JsonConverters
{
    public class IChannelJsonConverter : TypedAbstractJsonConverter<IChannel, ChannelType>
    {
        protected override string TypePropertyName => nameof(IChannel.Type);
        protected override IChannel GetObject(ChannelType type) => type switch
        {
            ChannelType.Email => new EmailChannel(),
            ChannelType.Telegram => new TelegramChannel(),
            ChannelType.Mattermost => new MattermostChannel(),
            ChannelType.Teams => new TeamsChannel(),
            _ => throw new NotImplementedException($"{type} {nameof(ChannelType)} is not implemented to convert to Json."),
        };
    }
}
