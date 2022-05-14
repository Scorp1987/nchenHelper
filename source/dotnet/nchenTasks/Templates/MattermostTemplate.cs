using nchen.Enums;

namespace nchen.Templates
{
    public class MattermostTemplate : ITemplate
    {
        public TemplateType Type => TemplateType.Mattermost;

        public string TemplateFilePath { get; set; }
    }
}
