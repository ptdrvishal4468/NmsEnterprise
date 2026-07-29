using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nms.Application.Common.Interfaces;
using Nms.Infrastructure.Security;

namespace Nms.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind JwtSettings section
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // Register Security Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}