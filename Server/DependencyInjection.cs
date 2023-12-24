using Application.Common;
using Domain.Common;
using Server.Hubs;
using Server.Repositories;

namespace Server;

public static class DependencyInjection
{
    public static IServiceCollection AddMonopolyServer(this IServiceCollection services)
    {
        services.AddSingleton<IRepository, InMemoryRepository>()
                .AddScoped<IEventBus<DomainEvent>, MonopolyEventBus>()
                .AddTransient(typeof(SignalrDefaultPresenter<>), typeof(SignalrDefaultPresenter<>));
        return services;
    }
}