using efcore_sandbox.Services;
using efcore_sandbox.Services.Interfaces;
using TasksScaffold.Models;

namespace TasksScaffold.Services;

public class SearchInTasksService
{
    public async Task<(int, int)> SearchInTasks(List<SimpleTask> tasks)
    {
        var cts = new CancellationTokenSource();
        var searchServices = new List<ISearchText>() { new QuickSearch(), new FuzzySearch(), new HeavySemanticSearch() };
        var results = await Task.WhenAll(searchServices.Select(async s =>
        {
            (int taskId, int score) result = (-1, 0);
            try
            {
                result = await s.SearchAsync("some text", cts.Token);
                if (!cts.IsCancellationRequested)
                {
                    await cts.CancelAsync();
                }

            }
            catch (TaskCanceledException _)
            {
            }

            return result;
        }).ToList());
        
        return results.MaxBy(tuple => tuple.score);
    }
}