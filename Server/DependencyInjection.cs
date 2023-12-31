using System.Reflection;
using Application.Common;
using Domain.Common;
using Server.Common;
using Server.Hubs;
using Server.Presenters;
using Server.Repositories;

namespace Server;

public static class DependencyInjection
{
    public static IServiceCollection AddMonopolyServer(this IServiceCollection services)
    {
        services.AddSingleton<IRepository, InMemoryRepository>()
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
