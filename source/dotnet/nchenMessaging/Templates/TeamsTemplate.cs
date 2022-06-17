using nchen.Messaging.Channels;

namespace nchen.Messaging.Templates
{
    public class TeamsTemplate : ITemplate
    {
        public ChannelType Type => ChannelType.Teams;
        public string TemplateFilePath { get; set; }
    }
}
