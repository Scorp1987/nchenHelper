using nchen.Enums;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class AssignTextTask : AGetDataTask
    {
        public override TaskType Type => TaskType.AssignText;
        public string Text { get; set; }
        protected override string FunctionString => Text == null ? "null" : $"'{Text}'";

        protected override Task<object> GetDataAsync() => Task.FromResult<object>(Text);
    }
}
