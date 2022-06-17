using System.Collections.Generic;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class AssignTextTask : AGetDataTask, ITask
    {
        public TaskType Type => TaskType.AssignText;
        public string Text { get; set; }


        protected override Task<object> GetDataAsync(Dictionary<string, object> data) => Task.FromResult<object>(Text);
        protected override string GetDataFunctionString(Dictionary<string, object> data) => Text == null ? "null" : $"'{Text}'";
    }
}
