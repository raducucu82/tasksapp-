using Autofac;
using TasksScaffold.Models;
using TasksScaffold.Services.Ifaces;

namespace TasksScaffold.Services;

public class CommandFactory
{
    private readonly Func<int, MarkCompletedTaskCommand> _completeTaskFunc;
    private readonly Func<SimpleTask, string, UpdateTaskCommand> _updateTaskFunc;

    public CommandFactory(
        Func<int, MarkCompletedTaskCommand> completeTaskFunc,
        Func<SimpleTask, string, UpdateTaskCommand> updateTaskFunc)
    {
        _completeTaskFunc = completeTaskFunc;
        _updateTaskFunc = updateTaskFunc;
    }

    public ITaskCommand BuildUpdateTaskCommand(SimpleTask task, string newDescription) => new UpdateTaskCommand(task, newDescription);

    public ITaskCommand BuildCompleteTaskCommand(int taskId) => _completeTaskFunc(taskId);
}