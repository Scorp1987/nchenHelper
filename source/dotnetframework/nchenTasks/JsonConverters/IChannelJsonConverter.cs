using nchen.Channels;
using nchen.Enums;
using Newtonsoft.Json.Converters;
using System;

namespace nchen.JsonConverters
{
    public class IChannelJsonConverter : TypedAbstractJsonConverter<IChannel, ChannelType>
    {
        protected override string TypePropertyName => nameof(IChannel.Type);
        protected override IChannel GetObject(ChannelType type)
        {
            switch (type)
            {
                case ChannelType.Email: return new EmailChannel();
                case ChannelType.Telegram: return new TelegramChannel();
                case ChannelType.Mattermost: return new MattermostChannel();
                case ChannelType.Teams: return new TeamsChannel();
                default: throw new NotImplementedException($"{type} {nameof(ChannelType)} is not implemented to convert to Json.");
            }
        }
    }
}
