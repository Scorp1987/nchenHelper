using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public abstract class ATask
    {
        public const int DEFAULT_TIMEOUT_MSEC = 10000;

        public virtual string Description { get; set; }
        public virtual int TimeoutMsec { get; set; } = DEFAULT_TIMEOUT_MSEC;


        public virtual async Task<RunResult> ExecuteAsync(Dictionary<string, object> data, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested) return RunResult.Cancel;
            var timeoutTask = Task.Delay(TimeoutMsec);
            var executeTask = ExecuteTaskAsync(data, cancellationToken);

            var finishTask = await Task.WhenAny(new Task[] { timeoutTask, executeTask });
            if (cancellationToken.IsCancellationRequested) return RunResult.Cancel;
            if (finishTask == timeoutTask)
                throw new TimeoutException($"Executing {GetFunctionString(data)} take more than {TimeoutMsec} millisecond");
            return await executeTask;
        }

        public abstract string GetFunctionString(Dictionary<string, object> data);
        protected abstract Task<RunResult> ExecuteTaskAsync(Dictionary<string, object> data, CancellationToken cancellationToken);

        //protected async Task<TResult> CheckTimeout<TResult>(Func<Task<TResult>> asyncFunc)
        //{
        //    var timeoutTask = Task.Delay(TimeoutMsec);
        //    var executeTask = asyncFunc();

        //    var finishTask = await Task.WhenAny(new Task[] { timeoutTask, executeTask });
        //    if (finishTask == timeoutTask)
        //        throw new TimeoutException($"Executing {ToString()} take more than {TimeoutMsec} millisecond");
        //    return await executeTask;
        //}
        //protected async Task CheckTimeout(Func<Task> asyncFunc)
        //{
        //    var timeoutTask = Task.Delay(TimeoutMsec);
        //    var executeTask = asyncFunc();

        //    var finishTask = await Task.WhenAny(new Task[] { timeoutTask, executeTask });
        //    if (finishTask == timeoutTask)
        //        throw new TimeoutException($"Executing {ToString()} take more than {TimeoutMsec} millisecond");
        //}
    }
}
