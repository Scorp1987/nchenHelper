using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Pipes
{
    public static class PipeHelper
    {
        public const int DEFAULT_TIMEOUT_MSEC = 300000;

        public static void WriteToNamedPipe(string pipeName, string json, int timeoutMsec = DEFAULT_TIMEOUT_MSEC)
        {
            using var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.Out);
            var waitTask = pipeServer.WaitForConnectionAsync();
            var timeoutTask = Task.Delay(timeoutMsec);
            var task = Task.WhenAny(waitTask, timeoutTask).Result;
            if (task == timeoutTask) throw new TimeoutException($"Waiting for '{pipeName}' is more than {timeoutMsec * 1000} sec");
            using var writer = new StreamWriter(pipeServer);
            writer.Write(json);
        }

        public static async Task WriteToNamedPipeAsync(string pipeName, string json, int timeoutMsec = DEFAULT_TIMEOUT_MSEC)
        {
            using var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.Out);
            var waitTask = pipeServer.WaitForConnectionAsync();
            var timeoutTask = Task.Delay(timeoutMsec);
            var task = await Task.WhenAny(waitTask, timeoutTask);
            if (task == timeoutTask) throw new TimeoutException($"Waiting for '{pipeName}' is more than {timeoutMsec * 1000} sec");
            using var writer = new StreamWriter(pipeServer);
            await writer.WriteAsync(json);
        }
    }
}
