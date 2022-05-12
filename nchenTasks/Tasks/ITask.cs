using nchen.Enums;
using nchen.JsonConverters;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    [JsonConverter(typeof(ITaskJsonConverter))]
    public interface ITask
    {
        TaskType Type { get; }
        string Description { get; set; }
        int TimeoutMsec { get; set; }

        Task<string> ExecuteAsync(Dictionary<string, object> data);
    }
}
