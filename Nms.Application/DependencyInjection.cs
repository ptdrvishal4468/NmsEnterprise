using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Nms.Application.Auth.Commands.Login;
using Nms.Application.Telemetry.Commands.PollDevice;
using Nms.Application.Telemetry.Commands.ProcessTelemetryData;
using Nms.Application.Telemetry.Queries.GetDeviceMetrics;
using Nms.Application.Users.Commands.CreateUser;
using Nms.Application.Users.Commands.UpdateUserRoles;
using Nms.Application.Users.Queries.GetUserById;
using Nms.Application.Users.Queries.GetUsersPaged;

namespace Nms.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        // Automatic FluentValidation registration
        services.AddValidatorsFromAssembly(assembly);

        // MediatR Registration
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Auth Command Handlers
        services.AddScoped<LoginCommandHandler>();

        // User Command & Query Handlers
        services.AddScoped<CreateUserCommandHandler>();
        services.AddScoped<UpdateUserRolesCommandHandler>();
        services.AddScoped<GetUserByIdQueryHandler>();
        services.AddScoped<GetUsersPagedQueryHandler>();

        // Telemetry Command & Query Handlers
        services.AddScoped<PollDeviceCommandHandler>();
        services.AddScoped<ProcessTelemetryDataCommandHandler>();
        services.AddScoped<GetDeviceMetricsQueryHandler>();

        return services;
    }
}