using nchen.Enums;
using System;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class GetDateTimeTask : AGetDataTask
    {
        public override TaskType Type => TaskType.GetDateTime;
        public string DateTimeString { get; set; }
        protected override string FunctionString
        {
            get
            {
                var parameterStr = string.IsNullOrEmpty(DateTimeString) ? "" : $"'{DateTimeString}'";
                return $"GetDateTime({parameterStr})";
            }
        }
        

        protected override Task<object> GetDataAsync()
        {
            if (string.IsNullOrEmpty(DateTimeString))
                return Task.FromResult<object>(DateTime.Now);
            else if (DateTime.TryParse(DateTimeString, out var dt))
                return Task.FromResult<object>(dt);
            else
                return Task.FromResult<object>(null);
        }
    }
}
