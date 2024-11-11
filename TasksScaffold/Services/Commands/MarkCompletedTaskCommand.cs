using TasksScaffold.Models;
using TasksScaffold.Services.Ifaces;

namespace TasksScaffold.Services;

public class MarkCompletedTaskCommand : ITaskCommand
{
    private readonly SimpleTask _task;

    public MarkCompletedTaskCommand(SimpleTask task)
    {
        _task = task;
    }
    
    public void Execute()
    {
        throw new NotImplementedException();
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }

    public string ToJson()
    {
        throw new NotImplementedException();
    }
}