using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class RunProcessTask : ATask, ITask
    {
        public TaskType Type => TaskType.RunProcess;
        public string FilePath { get; set; }
        public string Arguments { get; set; }
        public string StartIn { get; set; }


        public override string GetFunctionString(Dictionary<string, object> data) => $"RunProcess('{FilePath}','{Arguments}')";
        protected override async Task<RunResult> ExecuteTaskAsync(Dictionary<string, object> data, CancellationToken cancellationToken)
        {
            var startInfo = new ProcessStartInfo(FilePath, Arguments) { UseShellExecute = false };
            if (!string.IsNullOrEmpty(StartIn)) startInfo.WorkingDirectory = StartIn;
            var process = Process.Start(startInfo);
            await process.WaitForExitAsync(cancellationToken);
            return RunResult.Successful;
        }
    }
}
