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
            return type switch
            {
                TaskType.AssignText => new AssignTextTask(),
                TaskType.AssignDateTime => new AssignDateTimeTask(),
                TaskType.AssignNumber => new AssignNumberTask(),
                TaskType.AssignObject => new AssignObjectTask(),
                TaskType.SqlQuery => new SqlQueryTask(),
                TaskType.ListenNamedPipe => new ListenNamedPipeTask(),
                TaskType.SendToChannel => new SendToChannelTask(),
                TaskType.RunProcess => new RunProcessTask(),
                TaskType.SaveJsonObject => new SaveJsonObjectTask(),
                TaskType.ConcurrentTasks => new ConcurrentTasks(),
                TaskType.SequentialTasks => new SequentialTasks(),
                _ => throw new NotImplementedException($"{type} {nameof(TaskType)} is not implemented to convert to Json."),
            };
        }
    }
}
