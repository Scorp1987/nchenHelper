using nchen.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nchen.Tasks
{
    public class ConcurrentTasks : ATasks
    {
        public override TaskType Type => TaskType.ConcurrentTasks;


        public override async Task<string> ExecuteAsync(Dictionary<string, object> data)
        {
            try
            {
                var tasks = GetTasks(data);
                var stasks = new List<Task>(tasks.Length);
                foreach (var task in tasks)
                    stasks.Add(ExecuteTaskAsync(task, data));

                await Task.WhenAll(stasks);
            }
            catch(Exception ex)
            {
                Logger?.WriteException($"Unknown error occurred at {nameof(ConcurrentTasks.ExecuteAsync)}.", ex);
            }
            return null;
        }
        public override string ToString() => $"ConcurrentTasks()";
        private async Task ExecuteTaskAsync(ITask task, Dictionary<string, object> data)
        {
            var taskStartTime = DateTime.Now;
            var taskName = task.ToString();
            string result;
            string additionalResult = null;
            try
            {
                additionalResult = await task.ExecuteAsync(data);
                result = "Successful";
            }
            catch (Exception ex)
            {
                result = "Error";
                Logger?.WriteException(taskName, ex);
            }

            var taskTimeTaken = DateTime.Now - taskStartTime;
            if (!string.IsNullOrEmpty(additionalResult))
                additionalResult = $" {additionalResult}.";
            if (!(task is ATasks))
                await Logger?.WriteLogAsync($"Execute {taskName}...{result}.{additionalResult} TimeTaken: {taskTimeTaken.TotalSeconds:0.000} sec");
        }
    }
}
