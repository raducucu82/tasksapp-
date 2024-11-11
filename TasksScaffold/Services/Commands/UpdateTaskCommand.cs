using Newtonsoft.Json;
using TasksScaffold.Models;
using TasksScaffold.Services.Ifaces;

namespace TasksScaffold.Services;

[JsonObject(MemberSerialization.OptIn)]
public class UpdateTaskCommand : ITaskCommand
{
    private string _oldDescription;
    private readonly SimpleTask _task;
    private readonly string _newDescription;

    public UpdateTaskCommand(SimpleTask task, string newDescription)
    {
        _oldDescription = task.Description;
        _task = task;
        _newDescription = newDescription;
    }

    public void Execute()
    {
        _task.Description = _newDescription;
    }

    public void Undo()
    {
        _task.Description = _oldDescription;
    }

    [JsonProperty]
    public string OldDescription => _oldDescription;
    
    [JsonProperty]
    public string NewDescription => _newDescription;
    
    [JsonProperty]
    public int TaskId => _task.Id;

    public string ToJson() => JsonConvert.SerializeObject(this);

    public static ITaskCommand FromJson(string json) => JsonConvert.DeserializeObject<UpdateTaskCommand>(json); // todo
}