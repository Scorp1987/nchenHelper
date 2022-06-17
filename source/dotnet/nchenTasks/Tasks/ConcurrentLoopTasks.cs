using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class ConcurrentLoopTasks : ConcurrentTasks, ITask, ILoopTask
    {
        public override TaskType Type => TaskType.ConcurrentLoopTasks;
        public string Name { get; set; }
        public int? MaxConcurrentTasks { get; set; }

        public override async Task<RunResult> ExecuteAsync(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            var arr = this.GetArray(data);
            var allTasks = new List<Task<RunResult>>();
            if (MaxConcurrentTasks.HasValue)
            {
                using var throttler = new SemaphoreSlim(MaxConcurrentTasks.Value);
                foreach (var item in arr)
                {
                    await throttler.WaitAsync();
                    allTasks.Add(Task.Run(async () =>
                    {
                        try { return await ExecuteTaskAsync(data, item, cancellationToken); }
                        finally { throttler.Release(); }
                    }));
                }
            }
            else
            {
                foreach (var item in arr)
                    allTasks.Add(ExecuteTaskAsync(data, item, cancellationToken));
            }
            return await WhenAll(allTasks);
        }
        private async Task<RunResult> ExecuteTaskAsync(Dictionary<string, object> data, object item, CancellationToken cancellationToken)
        {
            var toPassData = new Dictionary<string, object>(data, StringComparer.InvariantCultureIgnoreCase);
            toPassData.SetDeepPropertyValue(Name, item);
            return await base.ExecuteAsync(toPassData, cancellationToken);
        }
    }
}
