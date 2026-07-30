using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
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


        // Command Handlers
        services.AddScoped<CreateUserCommandHandler>();
        services.AddScoped<UpdateUserRolesCommandHandler>();

        // Query Handlers
        services.AddScoped<GetUserByIdQueryHandler>();
        services.AddScoped<GetUsersPagedQueryHandler>();

        return services;
    }
}