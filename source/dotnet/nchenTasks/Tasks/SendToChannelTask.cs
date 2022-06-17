using nchen.Messaging.Channels;
using nchen.Messaging.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class SendToChannelTask : ATask, ITask, IChannelTask
    {
        private string _channelFilePath;


        public TaskType Type => TaskType.SendToChannel;
        public IChannel Channel { get; set; }
        public ITemplate Template { get; set; }
        public string ChannelFilePath
        {
            get => _channelFilePath;
            set
            {
                _channelFilePath = value;
                if (value == null) return;

                var json = File.ReadAllText(value);
                Channel = JsonConvert.DeserializeObject<IChannel>(json);
            }
        }


        public override string GetFunctionString(Dictionary<string, object> data) => $"SendTo{Channel.Type}('{Channel.Name}')";
        protected override async Task<RunResult> ExecuteTaskAsync(Dictionary<string, object> data, CancellationToken cancellaionToken)
        {
            data["channelType"] = Channel.Type.ToString();
            await Channel.SendAsync(Template, data);
            return RunResult.Successful;
        }
    }
}
