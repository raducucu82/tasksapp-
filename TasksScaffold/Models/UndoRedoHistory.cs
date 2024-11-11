namespace TasksScaffold.Models;

public class UndoRedoHistory
{
    public int Id { get; set; }
    
    public List<string> UndoStack { get; set; } = new List<string>();
    public List<string> RedoStack { get; set; } = new List<string>();
}