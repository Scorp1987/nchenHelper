using nchen.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public abstract class ATask : ITask
    {
        public const int DEFAULT_TIMEOUT_MSEC = 10000;

        public string Description { get; set; }
        public int TimeoutMsec { get; set; } = DEFAULT_TIMEOUT_MSEC;
        public abstract TaskType Type { get; }


        protected async Task<TResult> CheckTimeout<TResult>(Func<Task<TResult>> asyncFunc)
        {
            var timeoutTask = Task.Delay(TimeoutMsec);
            var executeTask = asyncFunc();

            var finishTask = await Task.WhenAny(new Task[] { timeoutTask, executeTask });
            if (finishTask == timeoutTask)
                throw new TimeoutException($"Executing {ToString()} take more than {TimeoutMsec} millisecond");
            return await executeTask;
        }
        protected async Task CheckTimeout(Func<Task> asyncFunc)
        {
            var timeoutTask = Task.Delay(TimeoutMsec);
            var executeTask = asyncFunc();

            var finishTask = await Task.WhenAny(new Task[] { timeoutTask, executeTask });
            if (finishTask == timeoutTask)
                throw new TimeoutException($"Executing {ToString()} take more than {TimeoutMsec} millisecond");
        }
        public abstract Task<string> ExecuteAsync(Dictionary<string, object> data);
        public abstract override string ToString();
    }
}
