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
    public class ReplyToChannelTask : ATask, ITask
    {
        [JsonConverter(typeof(ITemplateArrayJsonConverter))]
        public ITemplate[] Templates { get; set; }
        public TaskType Type => TaskType.ReplyToChannel;


        public override string GetFunctionString(Dictionary<string, object> data)
        {
            var channel = data.GetChannel();
            return $"ReplyTo{channel.Type}('{channel.Name}')";
        }
        protected override async Task<RunResult> ExecuteTaskAsync(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            var channel = data.GetChannel();
            if (!Templates.TryGetTemplate(channel.Type, out var template)) throw new InvalidDataException($"No template is avaliable for {nameof(channel.Type)}");
            await channel.SendAsync(template, data);
            return RunResult.Successful;
        }
    }
}
