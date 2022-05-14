using nchen.Enums;
using nchen.Templates;
using System;
using System.Threading.Tasks;

namespace nchen.Channels
{
    public class MattermostChannel : AChannel
    {
        public override ChannelType Type => ChannelType.Mattermost;

        public override Task<string> SendAsync(ITemplate template, object data) => throw new NotImplementedException();
    }
}
