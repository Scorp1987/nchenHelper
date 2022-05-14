using nchen.Enums;
using nchen.Tasks;
using Newtonsoft.Json.Converters;
using System;

namespace nchen.JsonConverters
{
    public class ITaskJsonConverter : TypedAbstractJsonConverter<ITask, TaskType>
    {
        protected override string TypePropertyName => nameof(ITask.Type);

        protected override ITask GetObject(TaskType type)
        {
            switch (type)
            {
                case TaskType.AssignText: return new AssignTextTask();
                case TaskType.AssignDateTime: return new AssignDateTimeTask();
                case TaskType.AssignNumber: return new AssignNumberTask();
                case TaskType.AssignJsonObject: return new AssignJsonObjectTask();
                case TaskType.SqlQuery: return new SqlQueryTask();
                case TaskType.ListenNamedPipe: return new ListenNamedPipeTask();

                case TaskType.SendToChannel: return new SendToChannelTask();

                case TaskType.RunProcess: return new RunProcessTask();
                case TaskType.SaveJsonObject: return new SaveJsonObjectTask();

                case TaskType.ConcurrentTasks: return new ConcurrentTasks();
                case TaskType.SequentialTasks: return new SequentialTasks();
                default: throw new NotImplementedException($"{type} {nameof(TaskType)} is not implemented to convert to Json.");
            }
        }
    }
}
