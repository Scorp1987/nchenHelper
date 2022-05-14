using nchen.Channels;
using nchen.Enums;
using Newtonsoft.Json.Converters;
using System;

namespace nchen.JsonConverters
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
