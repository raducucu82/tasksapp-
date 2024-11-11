using Autofac;
using TasksScaffold.Persistence;
using TasksScaffold.Services;
using TasksScaffold.Services.Ifaces;

namespace TasksScaffold;

public static class Setup
{
    public static void Register(ContainerBuilder builder)
    {
        builder.RegisterType<UpdateWithUndoRedoService>().AsSelf().SingleInstance();
        builder.RegisterType<UserSession>().AsSelf();
        builder.RegisterType<PersistenceService>().AsSelf().SingleInstance();
        builder.RegisterType<SearchInTasksService>().AsSelf().SingleInstance();
        
        builder.RegisterType<CommandFactory>().AsSelf().SingleInstance();
        builder.RegisterType<UpdateTaskCommand>().AsSelf().InstancePerRequest();
        builder.RegisterType<MarkCompletedTaskCommand>().AsSelf().InstancePerRequest();

    }
}