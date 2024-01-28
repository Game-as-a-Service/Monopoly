using Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMonopolyApplication(this IServiceCollection services)
    {
        // 依賴注入 Use Cases
        return services.AddUseCases();
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        var types = assembly.GetTypes();
        var useCaseType = typeof(CommandUsecase<,>);
        var queryUsecaseType = typeof(QueryUsecase<,>);

        foreach (var type in types.Where(t => t.BaseType?.IsGenericType is true && t.IsAbstract == false))
        {
            if (type.BaseType?.GetGenericTypeDefinition() == useCaseType)
            {
                services.AddTransient(type, type);
            }
            else if (type.BaseType?.GetGenericTypeDefinition() == queryUsecaseType)
            {
                services.AddTransient(type, type);
            }
        }

        return services;
    }
}