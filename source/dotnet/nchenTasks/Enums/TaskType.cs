namespace nchen.Enums
{
    public enum TaskType
    {
        AssignText = 101,
        AssignDateTime = 102,
        AssignNumber = 103,
        AssignObject = 104,
        SqlQuery = 106,
        ListenNamedPipe = 107,
        ReadDelimitedFile = 108,

        SendToChannel = 201,

        RunProcess = 304,
        SaveJsonObject = 305,

        ConcurrentTasks = 901,
        SequentialTasks = 902,
    }
}
