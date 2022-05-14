using System.Collections.Generic;
using System.IO;

namespace nchen.Tasks
{
    public abstract class ATasks : ATask
    {
        protected static ConsoleLogger Logger => TaskHelper.Logger;
        public string TasksFilePath { get; set; }
        public ITask[] Tasks { get; set; }


        protected virtual ITask[] GetTasks(Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(TasksFilePath))
                return Tasks ?? new ITask[0];
            else
                return TaskHelper.ExpandAdaptiveTemplate<ITask[]>(File.ReadAllText(TasksFilePath), data);
        }
    }
}
