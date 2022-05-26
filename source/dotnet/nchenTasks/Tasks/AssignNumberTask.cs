using nchen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class AssignNumberTask : AGetDataTask
    {
        public override TaskType Type => TaskType.AssignNumber;
        public decimal Value { get; set; }
        protected override string FunctionString => $"AssignNumber({Value})";


        protected override Task<object> GetDataAsync(Dictionary<string, object> data) => Task.FromResult<object>(Value);
    }
}
