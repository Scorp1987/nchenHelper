using nchen.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class RunProcessTask : ATask
    {
        public override TaskType Type => TaskType.RunProcess;
        public string FilePath { get; set; }
        public string Arguments { get; set; }
        public string StartIn { get; set; }


        public override Task<string> ExecuteAsync(Dictionary<string, object> data)
        {
            var startInfo = new ProcessStartInfo(FilePath, Arguments);
            if (!string.IsNullOrEmpty(StartIn)) startInfo.WorkingDirectory = StartIn;

            var process = Process.Start(startInfo);
            if (!process.WaitForExit(TimeoutMsec))
                throw new TimeoutException();
            return Task.FromResult<string>(null);
        }

        public override string ToString() => $"RunProcess('{FilePath}','{Arguments}')";
    }
}
