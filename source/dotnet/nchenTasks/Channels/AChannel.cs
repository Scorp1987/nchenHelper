using nchen.Enums;
using nchen.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nchen.Channels
{
    public abstract class AChannel : IChannel
    {
        public abstract ChannelType Type { get; }
        public abstract Task<string> SendAsync(ITemplate template, object data);

        public string Name { get; set; }
    }
}
