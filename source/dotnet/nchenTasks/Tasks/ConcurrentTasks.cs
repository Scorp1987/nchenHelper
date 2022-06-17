using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class ConcurrentTasks : ATasks, ITask
    {
        public virtual TaskType Type => TaskType.ConcurrentTasks;


        protected virtual async Task<RunResult> WhenAll(IEnumerable<Task<RunResult>> tasks)
        {
            foreach(var task in tasks)
            {
                var result = await task;
                if (result != RunResult.Successful) return result;
            }
            return RunResult.Successful;
        }
        public override async Task<RunResult> ExecuteAsync(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            var tasks = GetTasks(data);
            var stasks = new List<Task<RunResult>>(tasks.Length);
            foreach (var task in tasks)
                stasks.Add(ExecuteTaskAsync(task, data, cancellationToken));
            return await WhenAll(stasks);
        }
        protected override Task<RunResult> ExecuteTaskAsync(Dictionary<string, object> data, CancellationToken cancellationToken) => throw new System.NotImplementedException();
        private async Task<RunResult> ExecuteTaskAsync(ITask task, Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            if (task is ATasks || task is ReferenceTask)
                return await task.ExecuteAsync(data, cancellationToken);
            else if(task is AGetDataTask getDataTask)
            {
                var result = await Logger.RunSlowTaskAsync(task.GetFunctionString(data), () => task.ExecuteAsync(data, cancellationToken), r => (r == RunResult.Successful) ? getDataTask.GetSummaryResult(data) : null);
                return (result.Value == RunResult.Default) ? result.Key : result.Value;
            }
            else
            {
                var result = await Logger.RunSlowTaskAsync(task.GetFunctionString(data), () => task.ExecuteAsync(data, cancellationToken));
                return (result.Value == RunResult.Default) ? result.Key : result.Value;
            }
        }
    }
}
