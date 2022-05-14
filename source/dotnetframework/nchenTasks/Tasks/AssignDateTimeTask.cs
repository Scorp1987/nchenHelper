using nchen.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class AssignDateTimeTask : AGetDataTask
    {
        public override TaskType Type => TaskType.AssignDateTime;
        public string Value { get; set; }
        protected override string FunctionString
        {
            get
            {
                var parameterStr = string.IsNullOrEmpty(Value) ? "" : $"'{Value}'";
                return $"AssignDateTime({parameterStr})";
            }
        }
        

        protected override Task<object> GetDataAsync(Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(Value))
                return Task.FromResult<object>(DateTime.Now);
            else if (DateTime.TryParse(Value, out var dt))
                return Task.FromResult<object>(dt);
            else
                return Task.FromResult<object>(null);
        }
    }
}
