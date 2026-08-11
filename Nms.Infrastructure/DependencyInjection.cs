using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Interfaces;
using Nms.Infrastructure.Connectivity;
using Nms.Infrastructure.Data;
using Nms.Infrastructure.Data.Interceptors;
using Nms.Infrastructure.Data.Repositories;
using Nms.Infrastructure.Polling;
using Nms.Infrastructure.Security;
using Nms.Infrastructure.Snmp;
using Nms.Infrastructure.Ssh;
using Nms.Infrastructure.Telemetry;
using Nms.Infrastructure.Tenants;

namespace Nms.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 0. HttpContext & Multi-Tenant Core Services
        services.AddHttpContextAccessor();
        services.AddScoped<TenantContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<TenantContext>());
        services.AddScoped<ITenantResolver, HttpTenantResolver>();

        // 1. Interceptors
        services.AddScoped<AuditableEntityInterceptor>();

        // 2. DbContext - Standard SQL Server Registration with Transient Resilience
        services.AddDbContext<NmsDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(NmsDbContext).Assembly.GetName().Name);
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    })
                .AddInterceptors(interceptor);
        });

        // 3. Repositories & UnitOfWork
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IPollProfileRepository, PollProfileRepository>();
        services.AddScoped<IDeviceMetricRepository, DeviceMetricRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IReachabilityHistoryRepository, ReachabilityHistoryRepository>();
        services.AddScoped<IDiscoveryJobRepository, DiscoveryJobRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // 4. Bind JwtSettings
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // 5. Security Services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // 6. Telemetry, Polling & SNMP Infrastructure Registrations
        services.AddSingleton<IOidCatalog, OidCatalog>();
        services.AddTransient<ISnmpClientFactory, SnmpClientFactory>();
        services.AddScoped<ISnmpCollectorService, SnmpCollectorService>();
        services.AddScoped<ITelemetryEngine, TelemetryEngine>();
        services.AddSingleton<IPollingQueue, PollingQueue>();
        services.AddScoped<IPollScheduler, PollScheduler>();
        services.AddScoped<INetworkInterfaceRepository, NetworkInterfaceRepository>();

        // 7. Connectivity Services & Adapters
        services.AddTransient<IIcmpPingService, IcmpPingService>();
        services.AddTransient<IConnectionAdapter, IcmpConnectionAdapter>();
        services.AddTransient<IConnectionAdapter, SnmpConnectionAdapter>();
        services.AddTransient<IConnectionAdapterFactory, ConnectionAdapterFactory>();
        services.AddSingleton<ISshClientFactory, SshClientFactory>();
        services.AddTransient<IConnectionAdapter, SshConnectionAdapter>();

        return services;
    }
}