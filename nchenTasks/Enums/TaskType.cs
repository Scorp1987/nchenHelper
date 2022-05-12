namespace nchen.Enums
{
    public enum TaskType
    {
        AssignText = 101,
        GetDateTime = 102,
        SqlQuery = 103,
        ListenNamedPipe = 104,
        AssignJsonObject = 105,

        SendToChannel = 201,

        RunProcess = 304,
        SaveJsonObject = 305,

        ConcurrentTasks = 901,
        SequentialTasks = 902,
    }
}
