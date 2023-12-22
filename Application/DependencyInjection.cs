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
        var useCaseType = typeof(Usecase<>);
        var queryUsecaseType = typeof(QueryUsecase<,>);

        foreach (var type in types.Where(t => t.BaseType?.IsGenericType is true))
        {
            if (type.BaseType?.GetGenericTypeDefinition() == useCaseType)
            {
                services.AddScoped(type, type);
            }
            else if (type.BaseType?.GetGenericTypeDefinition() == queryUsecaseType)
            {
                services.AddScoped(type, type);
            }
        }

        return services;
    }
}