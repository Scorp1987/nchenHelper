using nchen.Messaging.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.Messaging.Templates
{
    public class EmailTemplate : ITemplate
    {
        public ChannelType Type => ChannelType.Email;
        public string Subject { get; set; }
        public string HeaderTemplateFilePath { get; set; }
        public string BodyTemplateFilePath { get; set; }
        public string FooterTemplateFilePath { get; set; }
    }
}
