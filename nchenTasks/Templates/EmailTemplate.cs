using nchen.Enums;

namespace nchen.Templates
{
    public class EmailTemplate : ITemplate
    {
        public TemplateType Type => TemplateType.Email;

        public string Subject { get; set; }

        public string HeaderTemplateFilePath { get; set; }

        public string BodyTemplateFilePath { get; set; }

        public string FooterTemplateFilePath { get; set; }
    }
}
