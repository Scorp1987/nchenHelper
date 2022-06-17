using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class AssignDateTimeTask : AGetDataTask, ITask
    {
        public TaskType Type => TaskType.AssignDateTime;
        public string Value { get; set; }


        protected override Task<object> GetDataAsync(Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(Value))
                return Task.FromResult<object>(DateTime.Now);
            else if (DateTime.TryParse(Value, out var dt))
                return Task.FromResult<object>(dt);
            else
                return Task.FromResult<object>(null);
        }
        protected override string GetDataFunctionString(Dictionary<string, object> data)
        {
            var parameterStr = string.IsNullOrEmpty(Value) ? "" : $"'{Value}'";
            return $"AssignDateTime({parameterStr})";
        }
    }
}
