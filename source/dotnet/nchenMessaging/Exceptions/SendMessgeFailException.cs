using nchen.Messaging.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace nchen.Messaging.Exceptions
{
    public class SendMessageFailException : Exception
    {
        public SendMessageFailException(IChannel channel, Exception innerException) : base($"Send message fail for {channel.Type}('{channel.Name}')", innerException) { }
    }
}
