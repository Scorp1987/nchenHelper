using nchen.Messaging.Channels;

namespace nchen.Tasks
{
    public interface IChannelTask
    {
        public IChannel Channel { get; set; }
        public string ChannelFilePath { get; set; }
    }
}
