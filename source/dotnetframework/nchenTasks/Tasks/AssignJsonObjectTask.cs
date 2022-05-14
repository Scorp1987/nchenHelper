using nchen.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class AssignJsonObjectTask : AGetDataTask
    {
        public override TaskType Type => TaskType.AssignJsonObject;
        public string FilePath { get; set; }
        public string JsonString { get; set; }
        protected override string FunctionString
        {
            get
            {
                var parameterStr = string.IsNullOrEmpty(FilePath) ? $"{nameof(JsonString)}:..." : $"{nameof(FilePath)}:'{FilePath}'";
                return $"AssignJsonObject({parameterStr})";
            }
        }


        protected override Task<object> GetDataAsync(Dictionary<string, object> data)
        {
            string json = string.IsNullOrEmpty(FilePath) ? JsonString : File.ReadAllText(FilePath);
            var obj = JsonConvert.DeserializeObject(json);
            return Task.FromResult(obj);
        }
    }
}
