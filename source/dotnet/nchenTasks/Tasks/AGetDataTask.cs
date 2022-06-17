using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public abstract class AGetDataTask : ATask
    {
        public string Name { get; set; }

        public override string GetFunctionString(Dictionary<string, object> data) => $"{Name}={GetDataFunctionString(data)}";
        public virtual string GetSummaryResult(object obj) => null;
        public string GetSummaryResult(Dictionary<string, object> data) => GetSummaryResult(data[Name]);
        protected override async Task<RunResult> ExecuteTaskAsync(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            var obj = await GetDataAsync(data);
            DisposeIfNecessary(data, obj);

            data[Name] = obj;
            return RunResult.Successful;
        }

        protected abstract Task<object> GetDataAsync(Dictionary<string, object> data);
        protected abstract string GetDataFunctionString(Dictionary<string, object> data);

        private void DisposeIfNecessary(Dictionary<string, object> data, object obj)
        {
            if (!data.TryGetValue(Name, out var srcObj)) return;
            if (!(srcObj is IDisposable disObj)) return;
            if (srcObj != obj) return;
            disObj.Dispose();
        }
    }
}
