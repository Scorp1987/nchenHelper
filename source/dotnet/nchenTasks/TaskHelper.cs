using AdaptiveCards;
using nchen.Messaging.Channels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public static class TaskHelper
    {
        private static ConsoleLogger _logger;
        public static ConsoleLogger Logger
        {
            get
            {
                if (_logger == null) _logger = new ConsoleLogger();
                return _logger;
            }
            set => _logger = value;
        }


        public static ITask ReadTask(string filePath, object data)
        {
            var json = File.ReadAllText(filePath);
            return AdaptiveCardHelper.ExpandAdaptiveTemplate<ITask>(json, data);
        }
        public static ITask[] ReadTasks(string filePath, object data)
        {
            var json = File.ReadAllText(filePath);
            return AdaptiveCardHelper.ExpandAdaptiveTemplate<ITask[]>(json, data);
        }


        public static IChannel GetChannel(this Dictionary<string, object> data)
        {
            if (!data.TryGetValue("channel", out var channelObj)) throw new InvalidOperationException("Can't reply to channel when channel is not found in data.");
            if (!(channelObj is IChannel channel)) throw new InvalidCastException($"channel in data is not {nameof(IChannel)}");
            return channel;
        }
        public static IChannel GetChannel(this IChannelTask task, Dictionary<string, object> data) => task.Channel ?? data.GetChannel();
    }
}
