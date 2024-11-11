using System.Data.Common;
using TasksScaffold.Models;

namespace TasksScaffold.Services;

public class UserSession
{
    private readonly PersistenceService _persistenceService;
    private readonly UpdateWithUndoRedoService _updateWithUndoRedoService;
    private readonly CommandFactory _commandFactory;
    private readonly string _userName;

    private readonly Lazy<Task<List<SimpleTask>>> _getTasksTask;
    private List<SimpleTask> _tasks;
    
    public delegate UserSession Factory(string userName);
    
    public UserSession(
        PersistenceService persistenceService, 
        UpdateWithUndoRedoService updateWithUndoRedoService, 
        CommandFactory commandFactory,
        string userName)
    {
        _persistenceService = persistenceService;
        _updateWithUndoRedoService = updateWithUndoRedoService;
        _commandFactory = commandFactory;
        _userName = userName;

        _tasks = null;
        _getTasksTask = new(async () =>
        {
            var tasks = await _persistenceService.GetAllTasksForUser(_userName);
            return tasks.ToList();
        });
    }

    public async Task<SimpleTask> GetTask(int taskId)
    {
        _tasks ??= await _getTasksTask.Value;
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);

        return task;
    }
    
    public async Task UpdateTask(int taskId, string newDescription)
    {
        _tasks ??= await _getTasksTask.Value;
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        
        if (task == null)
        {
            throw new MissingMemberException();
        }
        
        _updateWithUndoRedoService.ExecuteCommand(
            _commandFactory.BuildUpdateTaskCommand(task, newDescription));
    }

    public void Undo()
    {
        _updateWithUndoRedoService.Undo();
    }

    public void Redo()
    {
        _updateWithUndoRedoService.Redo();
    }
}