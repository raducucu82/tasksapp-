using Autofac;
using Microsoft.EntityFrameworkCore;
using TasksScaffold.Models;
using TasksScaffold.Persistence;
using TasksScaffold.Services;

namespace ConsoleApp1;

public class TestScenarioHelper
{
    // private readonly ApplicationDbContext _context;
    private readonly PersistenceService _persistenceService;
    private readonly UpdateWithUndoRedoService _updateWithUndoRedoService;
    private readonly UserSession.Factory _userSessionFactory;

    public TestScenarioHelper(
        PersistenceService persistenceService,
        // ApplicationDbContext context, 
        UpdateWithUndoRedoService updateWithUndoRedoService,
        UserSession.Factory userSessionFactory)
    {
        _persistenceService = persistenceService;
        _updateWithUndoRedoService = updateWithUndoRedoService;
        _userSessionFactory = userSessionFactory;
    }

    public async Task Run_TestScenario_TriggerThrottleDbUpdate()
    {
        Console.WriteLine("[] Seed test data");
        await using (var context = new ApplicationDbContext())
        {
             await Seed(context);
             await DumpDb(context);
        }
       
        Console.WriteLine("[] Update a task.");
        var userSession = _userSessionFactory.Invoke("user1");
        await userSession.UpdateTask(1, "Updated description.");

        Console.WriteLine("[] Wait for db save.");
        await Task.Delay(2000);
        await using (var context = new ApplicationDbContext())
        {
            await DumpDb(context);
        }
         
        Console.WriteLine("[] Undo task update and wait");
        userSession.Undo();
        
        Console.WriteLine("[] Wait for db save.");
        await Task.Delay(2000);
        await using (var context = new ApplicationDbContext())
        {
            await DumpDb(context);
        }

        Console.WriteLine("[] Chain two updates, with enough delay after the first update to trigger a db save.");
        userSession.Redo();
        await Task.Delay(500 + 100); // Throttle time + delay to start the db write;
        Console.WriteLine("[] First db update trigger should be canceled by the second.");
        await userSession.UpdateTask(1, "Overwrite the undo.");

        await Task.Delay(2000);
        await using (var context = new ApplicationDbContext())
        {
            await DumpDb(context);
        }
        
        await _persistenceService.WaitForPendingUpdatesToComplete();
    }

    public async Task RunSimple()
    {
        await using (var context = new ApplicationDbContext())
        {
            await Seed(context);
            await DumpDb(context);
        }

        await using (var context = new ApplicationDbContext())
        {
            var user = await context.Users
                .Include(user => user.Tasks)
                .FirstOrDefaultAsync(user => user.Username == "user1");

            user.Tasks.First().Description = "Updated now.";
            await context.SaveChangesAsync();
        }

        // var persistenceService = new PersistenceService();
        var tasks = await _persistenceService.GetAllTasksForUser("user2");
        tasks.First().Description = "Updated second...";
        _persistenceService.NotifyPersistData();
        await Task.Delay(2000);
        // await persistenceService.Context.SaveChangesAsync();
        
        await using (var context2 = new ApplicationDbContext())
        {
            await DumpDb(context2);
        }
        
        await _persistenceService.WaitForPendingUpdatesToComplete();
    }
    
    public async Task Seed(ApplicationDbContext context)
    {
        // await using var _context = new ApplicationDbContext();
        
        // Seed db with some users and tasks. 
        var user1 = new User
        {
            Username = "user1",
            Email = "user1@example.com",
            Tasks = new List<SimpleTask>()
            {
                new SimpleTask()
                {
                    Title = "Task 1 for User 1",
                    Description = "Description for Task 1",
                    DueDate = DateTime.Now.AddDays(7),
                    IsCompleted = false                                  
                }
            }
        };
        
        var user2 = new User
        {
            Username = "user2",
            Email = "user2@example.com",
            Tasks = new List<SimpleTask>()
            {
                new SimpleTask() 
                {
                    Title = "Task 1 for User 2",
                    Description = "Description for Task 1",
                    DueDate = DateTime.Now.AddDays(5),
                    IsCompleted = false
                },
                new SimpleTask() 
                {
                    Title = "Task 2 for User 2",
                    Description = "Description for Task 2",
                    DueDate = DateTime.Now.AddDays(10),
                    IsCompleted = false
                }
            }
        };
        context.Users.AddRange(user1, user2);
        await context.SaveChangesAsync();

        // Add some undo/redos'
        // var task = await context.Tasks
        //     .FirstOrDefaultAsync(t => t.Id == 1);
        // _updateWithUndoRedoService.ExecuteCommand(new UpdateTaskCommand(task, "First Update."));
        // _updateWithUndoRedoService.ExecuteCommand(new UpdateTaskCommand(task, "Second Update."));
        // await context.SaveChangesAsync();
    }
    
    public async Task DumpDb(ApplicationDbContext context)
    {
        // await using var _context = new ApplicationDbContext();
        Console.WriteLine($">> DB Snapshot {DateTime.Now}");
        await context.Users.Include(user => user.Tasks).ForEachAsync(user =>
        {
            Console.WriteLine($"- {user.Username} (:{user.Id})");
            foreach (var task in user.Tasks)
            {
                Console.WriteLine($"\t- {task.Description} (:{task.Id})");
            }
        });
        Console.WriteLine("<<END");
    }
}