using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ObjectExtension
    {
        public static void WriteToNamedPipe<T>(this T @object, string pipeName)
        {
            var json = JsonConvert.SerializeObject(@object, new StringEnumConverter());
            PipeHelper.WriteToNamedPipe(pipeName, json);
        }
        public static async Task WriteToNamedPipeAsync<T>(this T @object, string pipeName)
        {
            var json = JsonConvert.SerializeObject(@object);
            await PipeHelper.WriteToNamedPipeAsync(pipeName, json);
        }
    }
}
