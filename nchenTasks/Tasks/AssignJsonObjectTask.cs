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
        protected override string FunctionString => $"AssignJsonObject('{FilePath}')";


        protected override Task<object> GetDataAsync()
        {
            var json = File.ReadAllText(FilePath);
            var obj = JsonConvert.DeserializeObject(json);
            return Task.FromResult(obj);
        }
    }
}
