using nchen.Tasks.JsonConverters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    [JsonConverter(typeof(ITaskJsonConverter))]
    public interface ITask
    {
        TaskType Type { get; }
        string Description { get; set; }
        int TimeoutMsec { get; set; }


        Task<RunResult> ExecuteAsync(Dictionary<string, object> data, CancellationToken cancellationToken = default);
        string GetFunctionString(Dictionary<string, object> data);
    }
}
