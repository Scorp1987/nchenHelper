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
    public class SaveJsonObjectTask : ATask
    {
        public override TaskType Type => TaskType.SaveJsonObject;
        public string Data { get; set; }
        public string FilePath { get; set; }

        public override async Task<string> ExecuteAsync(Dictionary<string, object> data)
        {
            object toWrite;
            if (string.IsNullOrEmpty(Data))
                toWrite = data;
            else
                toWrite = data[Data];

            var json = JsonConvert.SerializeObject(toWrite);
            using (var stream = File.Open(FilePath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var writer = new StreamWriter(stream))
                await writer.WriteAsync(json);
            return null;
        }

        public override string ToString()
        {
            var parameterStr = string.IsNullOrEmpty(Data) ? "" : $", Data:'{Data}'";
            parameterStr += $", FilePath:'{FilePath}'";
            return $"SaveJsonObject({parameterStr.Substring(2)})";
        }
    }
}
