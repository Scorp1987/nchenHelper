using nchen.Messaging.Templates;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Messaging.Channels
{
    public class MattermostChannel : AChannel, IChannel
    {
        public ChannelType Type => ChannelType.Mattermost;


        public Task<object> SendAsync(ITemplate template, object data) => throw new NotImplementedException();
    }
}
