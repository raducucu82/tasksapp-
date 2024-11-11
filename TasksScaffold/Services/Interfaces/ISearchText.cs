namespace efcore_sandbox.Services.Interfaces;

public interface ISearchText
{
    /// <returns>TaskId + Score</returns>
    Task<(int, int)> SearchAsync(string text, CancellationToken ct);
}