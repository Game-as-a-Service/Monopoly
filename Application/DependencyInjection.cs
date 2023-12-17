using Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddMonopolyApplication(this IServiceCollection services)
    {
        // 依賴注入 Use Cases
        services.AddUseCases();

        return services;
    }

    private static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        var types = assembly.GetTypes();
        var useCaseType = typeof(Usecase<>);
        var queryUsecaseType = typeof(QueryUsecase<,>);

        foreach (var type in types)
        {
            if (type.BaseType?.IsGenericType == true && type.BaseType?.GetGenericTypeDefinition() == useCaseType)
            {
                services.AddScoped(type, type);
            }
            else if (type.BaseType?.IsGenericType == true && type.BaseType?.GetGenericTypeDefinition() == queryUsecaseType)
            {
                services.AddScoped(type, type);
            }
        }

        return services;
    }
}