using System.Collections.Generic;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class AssignNumberTask : AGetDataTask, ITask
    {
        public TaskType Type => TaskType.AssignNumber;
        public decimal Value { get; set; }


        protected override Task<object> GetDataAsync(Dictionary<string, object> data) => Task.FromResult<object>(Value);
        protected override string GetDataFunctionString(Dictionary<string, object> data) => $"AssignNumber({Value})";
    }
}
