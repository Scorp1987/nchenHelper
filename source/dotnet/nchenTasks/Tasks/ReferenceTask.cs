using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class ReferenceTask : ATask, ITask
    {
        public TaskType Type => TaskType.ReferenceTask;
        public string FilePath { get; set; }


        public override async Task<RunResult> ExecuteAsync(Dictionary<string, object> data, CancellationToken cancellationToken = default)
        {
            var json = File.ReadAllText(FilePath);
            var task = AdaptiveCardHelper.ExpandAdaptiveTemplate<ITask>(json, data);
            return await task.ExecuteAsync(data, cancellationToken);
        }
        public override string GetFunctionString(Dictionary<string, object> data) => $"ReferenceTask('{FilePath}')";
        protected override Task<RunResult> ExecuteTaskAsync(Dictionary<string, object> data, CancellationToken cancellationToken) => throw new NotImplementedException();
    }
}
