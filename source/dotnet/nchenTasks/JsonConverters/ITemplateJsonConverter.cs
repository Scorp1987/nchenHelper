using nchen.Enums;
using nchen.Templates;
using Newtonsoft.Json.Converters;
using System;

namespace nchen.JsonConverters
{
    public class ITemplateJsonConverter : TypedAbstractJsonConverter<ITemplate, TemplateType>
    {
        protected override string TypePropertyName => nameof(ITemplate.Type);
        protected override ITemplate GetObject(TemplateType type)
        {
            switch (type)
            {
                case TemplateType.Email: return new EmailTemplate();
                case TemplateType.Telegram: return new TelegramTemplate();
                case TemplateType.Mattermost: return new MattermostTemplate();
                case TemplateType.Teams: return new TeamsTemplate();
                default: throw new NotImplementedException($"{type} {nameof(TemplateType)} is not implemented to convert to Json.");
            }
        }
    }
}
