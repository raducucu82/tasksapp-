using Autofac;
using Newtonsoft.Json;
using TasksScaffold.Models;
using TasksScaffold.Persistence;
using TasksScaffold.Services.Ifaces;

namespace TasksScaffold.Services;

public class UpdateWithUndoRedoService
{
    private readonly Stack<ITaskCommand> _undoStack = new Stack<ITaskCommand>();
    private readonly Stack<ITaskCommand> _redoStack = new Stack<ITaskCommand>();
    private readonly PersistenceService _persistence;

    public UpdateWithUndoRedoService(PersistenceService persistence)
    {
        _persistence = persistence;
    }

    public void ExecuteCommand(ITaskCommand command)
    {
        command.Execute();
        _undoStack.Push(command);
        
        SaveUndoRedoHistory();
    }

    public void Undo()
    {
        if (_undoStack.TryPop(out var command))
        {
            command.Undo();
            _redoStack.Push(command);
            
            SaveUndoRedoHistory();
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            var command = _redoStack.Pop();
            command.Execute();
            _undoStack.Push(command);
            SaveUndoRedoHistory();
        }
    }

    private void SaveUndoRedoHistory()
    {
        // Simple bulk save
        // TODO save deltas? - 
        var history = new UndoRedoHistory
        {
            UndoStack = _undoStack.Select(command => command.ToJson()).ToList(),
            RedoStack = _redoStack.Select(command => command.ToJson()).ToList()
        };
        _persistence.UpdateUndoRedoHistory(history);
    }

    public void LoadUndoRedoHistory()
    {
        // todo
    }
}