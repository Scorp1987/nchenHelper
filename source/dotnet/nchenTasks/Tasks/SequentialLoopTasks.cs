using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class SequentialLoopTasks : SequentialTasks, ITask, ILoopTask
    {
        public override TaskType Type => TaskType.SequentialLoopTasks;
        public string Name { get; set; }


        public override async Task<RunResult> ExecuteAsync(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            var arr = this.GetArray(data);
            var toPassData = new Dictionary<string, object>(data, StringComparer.InvariantCultureIgnoreCase);
            foreach(var item in arr)
            {
                toPassData.SetDeepPropertyValue(Name, item);
                var result = await base.ExecuteAsync(toPassData, cancellationToken);
                if (StopIfError && result != RunResult.Successful) return result;
            }
            return RunResult.Successful;
        }
    }
}
