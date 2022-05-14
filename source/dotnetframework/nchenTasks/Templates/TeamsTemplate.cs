using nchen.Enums;

namespace nchen.Templates
{
    public class TeamsTemplate : ITemplate
    {
        public TemplateType Type => TemplateType.Teams;
        public string TemplateFilePath { get; set; }
    }
}
