using nchen.Enums;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class ListenNamedPipeTask : AGetDataTask
    {
        public override TaskType Type => TaskType.ListenNamedPipe;
        public string PipeName { get; set; }
        protected override string FunctionString => $"ListenNamedPipe('{PipeName}')";


        protected override async Task<object> GetDataAsync(Dictionary<string, object> data)
        {
            using (var pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.In))
            {
                await pipeClient.ConnectAsync(TimeoutMsec);
                using (var reader = new StreamReader(pipeClient))
                {
                    string json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject(json);
                }
            }
        }
    }
}
