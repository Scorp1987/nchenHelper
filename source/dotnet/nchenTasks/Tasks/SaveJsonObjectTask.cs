using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class SaveJsonObjectTask : ATask, ITask
    {
        public TaskType Type => TaskType.SaveJsonObject;
        public string Name { get; set; }
        public string FilePath { get; set; }


        public override string GetFunctionString(Dictionary<string, object> data)
        {
            var parameterStr = string.IsNullOrEmpty(Name) ? "" : $", Name:'{Name}'";
            parameterStr += $", FilePath:'{FilePath}'";
            return $"SaveJsonObject({parameterStr[2..]})";
        }
        protected override async Task<RunResult> ExecuteTaskAsync(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            object toWrite = string.IsNullOrEmpty(Name) ? data : data[Name];
            var json = JsonConvert.SerializeObject(toWrite);
            using (var stream = File.Open(FilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var writer = new StreamWriter(stream))
                await writer.WriteAsync(json);
            return RunResult.Successful;
        }
    }
}
