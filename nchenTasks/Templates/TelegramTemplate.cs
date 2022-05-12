using nchen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nchen.Templates
{
    public class TelegramTemplate : ITemplate
    {
        public TemplateType Type => TemplateType.Telegram;

        public string TemplateFilePath { get; set; }
    }
}
