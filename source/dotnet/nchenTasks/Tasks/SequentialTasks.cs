using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class SequentialTasks : ATasks, ITask
    {
        public virtual TaskType Type => TaskType.SequentialTasks;
        public bool StopIfError { get; set; }


        public override async Task<RunResult> ExecuteAsync(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            var tasks = GetTasks(data);
            foreach (var task in tasks)
            {
                var result = await ExecuteTaskAsync(task, data, cancellationToken);
                if (StopIfError && result != RunResult.Successful) return result;
            }
            return RunResult.Successful;
        }
        protected override Task<RunResult> ExecuteTaskAsync(Dictionary<string, object> data, CancellationToken cancellationToken) => throw new NotImplementedException();
        private async Task<RunResult> ExecuteTaskAsync(ITask task, Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            if (task is ATasks || task is ReferenceTask)
                return await task.ExecuteAsync(data, cancellationToken);
            else if (task is AskInputsFromChannelTask)
            {
                var result = await Logger.RunSlowTaskAsync(task.GetFunctionString(data), () => task.ExecuteAsync(data, cancellationToken));
                return (result.Value == RunResult.Default) ? result.Key : result.Value;
            }
            else if (task is AGetDataTask getDataTask)
            {
                var result = await Logger.RunFastTaskAsync(task.GetFunctionString(data), () => task.ExecuteAsync(data, cancellationToken), r => (r == RunResult.Successful) ? getDataTask.GetSummaryResult(data) : null);
                return (result.Value == RunResult.Default) ? result.Key : result.Value;
            }
            else
            {
                var result = await Logger.RunFastTaskAsync(task.GetFunctionString(data), () => task.ExecuteAsync(data, cancellationToken));
                return (result.Value == RunResult.Default) ? result.Key : result.Value;
            }
        }
    }
}
