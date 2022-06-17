using nchen.Messaging.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.Messaging.Templates
{
    public class MattermostTemplate : ITemplate
    {
        public ChannelType Type => ChannelType.Mattermost;
        public string TemplateFilePath { get; set; }
    }
}
