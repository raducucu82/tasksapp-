using efcore_sandbox.Services.Interfaces;

namespace efcore_sandbox.Services;

public class QuickSearch: ISearchText 
{
    public async Task<(int, int)> SearchAsync(string text, CancellationToken ct)
    {
        Console.WriteLine("[] Start SimpleSearch");
        await Task.Delay(TimeSpan.FromSeconds(2), ct);
        Console.WriteLine("[] Done Simple search.");

        return (1, 5);
    }
}

public class FuzzySearch : ISearchText
{
    public async Task<(int, int)> SearchAsync(string text, CancellationToken ct)
    {
        Console.WriteLine("Start Fuzzy search");
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), ct);
            Console.WriteLine("Done Fuzzy Search.");
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"{nameof(FuzzySearch)}:{e.Message}");
            throw;
        }

        return (-1, 0);
    }
}

public class HeavySemanticSearch: ISearchText // the must-have AI cameo
{
    public async Task<(int, int)> SearchAsync(string text, CancellationToken ct)
    {
        Console.WriteLine("[] Start SemanticSearch");
        await Task.Delay(TimeSpan.FromSeconds(100), ct);
        Console.WriteLine("[] Done SemanticSearch.");

        return (2, 5);
    }
}
