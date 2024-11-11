using Autofac;
using Microsoft.EntityFrameworkCore;
using TasksScaffold.Services;

namespace ConsoleApp1;

abstract class Program
{
    static async Task Main(string[] args)
    {
        var container = RegisterServices();
        await using (var scope = container.BeginLifetimeScope())
        {
            var seedTestData = scope.Resolve<TestScenarioHelper>();
            await seedTestData.Run_TestScenario_TriggerThrottleDbUpdate();
            // await seedTestData.RunSimple();
        }
    }

    static IContainer RegisterServices()
    {
        var builder = new ContainerBuilder();
        TasksScaffold.Setup.Register(builder);
        builder.RegisterType<TestScenarioHelper>().AsSelf();

        return builder.Build();
    }
}