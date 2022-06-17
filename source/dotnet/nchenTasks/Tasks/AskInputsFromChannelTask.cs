using nchen.Messaging.Channels;
using nchen.Messaging.JsonConverters;
using nchen.Messaging.Templates;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class AskInputsFromChannelTask : AGetDataTask, ITask, IChannelTask
    {
        private string _channelFilePath;


        [JsonConverter(typeof(ITemplateArrayJsonConverter))]
        public ITemplate[] Templates { get; set; }

        [JsonConverter(typeof(ITemplateArrayJsonConverter))]
        public ITemplate[] TimeoutTemplates { get; set; }


        public TaskType Type => TaskType.AskInputsFromChannel;
        public IChannel Channel { get; set; }
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
        private CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();


        public override async Task<RunResult> ExecuteAsync(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            try
            {
                var result = await base.ExecuteAsync(data, cancellationToken);
                if (result == RunResult.Cancel) CancellationTokenSource.Cancel();
                return result;
            }
            catch (TimeoutException ex)
            {
                CancellationTokenSource.Cancel();
                await SendTimeoutMessageAsync(data);
                throw ex;
            }
        }
        protected override async Task<object> GetDataAsync(Dictionary<string, object> data)
        {
            var channel = this.GetChannel(data);
            var template = Templates.GetTemplate(channel.Type);
            return await channel.AskInputsAsync(template, data, CancellationTokenSource.Token);
        }
        protected override string GetDataFunctionString(Dictionary<string, object> data)
        {
            IChannel channel = this.GetChannel(data);
            return $"AskInputFrom{channel?.Type}('{channel?.Name}')";
        }


        private async Task SendTimeoutMessageAsync(Dictionary<string, object> data)
        {
            var channel = this.GetChannel(data);
            var template = TimeoutTemplates.GetTemplate(channel.Type);
            await channel.SendAsync(template, data);
        }
    }
}
