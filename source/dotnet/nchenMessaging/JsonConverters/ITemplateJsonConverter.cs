using nchen.Messaging.Channels;
using nchen.Messaging.Templates;
using Newtonsoft.Json.Converters;
using System;

namespace nchen.Messaging.JsonConverters
{
    public class ITemplateJsonConverter : TypedAbstractJsonConverter<ITemplate, ChannelType>
    {
        protected override string TypePropertyName => nameof(ITemplate.Type);
        protected override ITemplate GetObject(ChannelType type)
        {
            return type switch
            {
                ChannelType.Email => new EmailTemplate(),
                ChannelType.Telegram => new TelegramTemplate(),
                ChannelType.Mattermost => new MattermostTemplate(),
                ChannelType.Teams => new TeamsTemplate(),
                _ => throw new NotImplementedException($"{type} {nameof(ChannelType)} is not implemented to convert to Json."),
            };
        }
    }
}
