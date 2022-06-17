using nchen.Messaging.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nchen.Messaging.Templates
{
    public class TelegramTemplate : ITemplate
    {
        public ChannelType Type => ChannelType.Telegram;
        public string TemplateFilePath { get; set; }
        public string InlineDataPrefix { get; set; }
    }
}
