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
    public class AssignObjectTask : AGetDataTask
    {
        public override TaskType Type => TaskType.AssignObject;
        public string FilePath { get; set; }
        public string Json { get; set; }
        public object Value{ get; set; }
        protected override string FunctionString
        {
            get
            {
                string parameterStr;
                if (Value != null) parameterStr = "Value:{}";
                else if (string.IsNullOrEmpty(FilePath)) parameterStr = $"FilePath:'{FilePath}'";
                else if (string.IsNullOrEmpty(Json)) parameterStr = $"Json:'{Json}'";
                else parameterStr = "";
                return $"AssignJsonObject({parameterStr})";
            }
        }


        protected override Task<object> GetDataAsync(Dictionary<string, object> data)
        {
            if (Value != null)
                return Task.FromResult(Value);

            string json = string.IsNullOrEmpty(FilePath) ? Json : File.ReadAllText(FilePath);
            var obj = JsonConvert.DeserializeObject(json);
            return Task.FromResult(obj);
        }
    }
}
