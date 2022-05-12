using nchen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class SequentialTasks : ATasks
    {
        public override TaskType Type => TaskType.SequentialTasks;


        public override async Task<string> ExecuteAsync(Dictionary<string, object> data)
        {
            var tasks = GetTasks(data);
            foreach (var task in tasks)
                await task.RunAsync(data);
            return null;
        }
        public override string ToString() => $"SequencialTasks()";
    }
}
