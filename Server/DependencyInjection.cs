using Application.Common;
using Domain.Common;
using Server.Common;
using Server.Presenters;
using Server.Repositories;
using System.Reflection;

namespace Server;

public static class DependencyInjection
{
    public static IServiceCollection AddMonopolyServer(this IServiceCollection services)
    {
        var repository = new InMemoryRepository();
        services.AddSingleton<ICommandRepository>(repository)
                .AddSingleton<IQueryRepository>(repository)
                .AddSingleton<IEventBus<DomainEvent>, MonopolyEventBus>()
                .AddTransient(typeof(SignalrDefaultPresenter<>), typeof(SignalrDefaultPresenter<>));
        services.AddSignalREventHandlers();
        return services;
    }

    private static IServiceCollection AddSignalREventHandlers(this IServiceCollection services)
    {
        var handlers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false }
                          && t.IsAssignableTo(typeof(IMonopolyEventHandler)))
            .ToList();
        foreach (var handler in handlers)
        {
            services.AddSingleton(typeof(IMonopolyEventHandler), handler);
        }
        return services;
    }
}
