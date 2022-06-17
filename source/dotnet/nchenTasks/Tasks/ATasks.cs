using System.Collections.Generic;
using System.IO;

namespace nchen.Tasks
{
    public abstract class ATasks : ATask
    {
        protected static ConsoleLogger Logger => TaskHelper.Logger;
        public ITask[] Tasks { get; set; }
        public bool IsLog { get; set; }


        public override string GetFunctionString(Dictionary<string, object> data) => null;
        protected virtual ITask[] GetTasks(Dictionary<string, object> data) => Tasks ?? new ITask[0];
    }
}
