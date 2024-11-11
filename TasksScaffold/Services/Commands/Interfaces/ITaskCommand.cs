using TasksScaffold.Models;

namespace TasksScaffold.Services.Ifaces;

public interface ITaskCommand
{
    void Execute();
    
    void Undo();
    
    public string ToJson();
}