using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace System.IO.Pipes
{
    public static class PipeHelper
    {
        public static void WriteToNamedPipe(string pipeName, string json)
        {
            using var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.Out);
            pipeServer.WaitForConnection();
            using var writer = new StreamWriter(pipeServer);
            writer.Write(json);
        }

        public static async Task WriteToNamedPipeAsync(string pipeName, string json)
        {
            using var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.Out);
            await pipeServer.WaitForConnectionAsync();
            using var writer = new StreamWriter(pipeServer);
            await writer.WriteAsync(json);
        }
    }
}
