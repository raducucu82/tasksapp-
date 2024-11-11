using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TasksScaffold.Models;
using TasksScaffold.Persistence;

namespace TasksScaffold.Services;

public class PersistenceService
{
    private readonly ApplicationDbContext _context;
    private readonly Subject<int> _pendingUpdates;
    
    private readonly Task _throttleTask;

    public PersistenceService()
    {
        _context = new ApplicationDbContext();
        _pendingUpdates = new();
        
        _throttleTask = SpinUpThrottle();
    }

    public ApplicationDbContext Context => _context;

    private Task SpinUpThrottle()
    {
        var sem = new SemaphoreSlim(1, 1);
        CancellationTokenSource cts = null;
        
        var rx = _pendingUpdates
            .ObserveOn(TaskPoolScheduler.Default)
            .Throttle(TimeSpan.FromMilliseconds(500))
            .Select(async _ =>
            {
                cts?.Cancel();
                cts = new CancellationTokenSource();

                try
                {
                    await sem.WaitAsync(cts.Token);
                    await PersistDataAsyncLongProcessing(cts.Token);
                }
                catch (TaskCanceledException __)
                {
                }
                finally
                {
                    sem.Release();
                }
                
            })
            .Select(t => t.ToObservable())
            .Concat();

        return rx.RunAsync(CancellationToken.None).ToTask();
    }
        

    public async Task<ICollection<SimpleTask>> GetAllTasksForUser(string userName)
    {
        var user = await _context.Users
            .Include(user => user.Tasks)
            .FirstOrDefaultAsync(user => user.Username == userName);

        return user?.Tasks;
    }

    public void UpdateUndoRedoHistory(UndoRedoHistory updates)
    {
        _context.UndoRedoHistories.Add(updates);
        NotifyPersistData();
    }

    public void NotifyPersistData()
    {
        _pendingUpdates.OnNext(0);
    }
    
    public async Task PersistDataAsyncLongProcessing(CancellationToken ct)
    {
        Console.WriteLine(" -> Start write to DB...");
        try
        {
            await Task.Delay(200); // simulate non-cancellable part
            await Task.Delay(1000, ct);
            await _context.SaveChangesAsync(ct);
        }
        catch (TaskCanceledException _)
        {
            Console.WriteLine(" -> Canceled.");
            throw;
        }
        Console.WriteLine(" -> End write to DB.");
    }

    public async Task WaitForPendingUpdatesToComplete()
    {
        _pendingUpdates.OnCompleted();
        await _throttleTask;
    }
}