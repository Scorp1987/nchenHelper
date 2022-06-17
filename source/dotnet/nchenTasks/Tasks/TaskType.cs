namespace nchen.Tasks
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
        AskInputsFromChannel = 202,
        ReplyToChannel = 203,

        RunProcess = 304,
        SaveJsonObject = 305,

        ConcurrentTasks = 901,
        SequentialTasks = 902,
        ConcurrentLoopTasks = 903,
        SequentialLoopTasks = 904,
        ReferenceTask = 999
    }
}
