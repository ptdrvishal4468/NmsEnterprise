using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Nms.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}