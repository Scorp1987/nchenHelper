using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public abstract class AGetDataTask : ATask
    {
        public string Name { get; set; }
        protected abstract string FunctionString { get; }

        public override async Task<string> ExecuteAsync(Dictionary<string, object> data)
        {
            var obj = await CheckTimeout(GetDataAsync);

            if (data.TryGetValue(Name, out var srcObj)
                && srcObj is IDisposable srcdispObj
                && srcdispObj != obj)
                srcdispObj.Dispose();

            data[Name] = obj;
            return GetResult(obj);
        }
        public override string ToString() => $"{Name}={FunctionString}";

        protected virtual string GetResult(object obj) => null;
        protected abstract Task<object> GetDataAsync();
    }
}
