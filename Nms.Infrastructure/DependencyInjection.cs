using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Interfaces;
using Nms.Infrastructure.Caching;
using Nms.Infrastructure.ConfigurationBackups;
using Nms.Infrastructure.Connectivity;
using Nms.Infrastructure.Cybersecurity.Adapters;
using Nms.Infrastructure.Data;
using Nms.Infrastructure.Data.Interceptors;
using Nms.Infrastructure.Data.Repositories;
using Nms.Infrastructure.Notifications.Options;
using Nms.Infrastructure.Notifications.Providers;
using Nms.Infrastructure.Polling;
using Nms.Infrastructure.Security;
using Nms.Infrastructure.Snmp;
using Nms.Infrastructure.Snmp.Options;
using Nms.Infrastructure.Ssh;
using Nms.Infrastructure.Syslog;
using Nms.Infrastructure.Syslog.Options;
using Nms.Infrastructure.Telemetry;
using Nms.Infrastructure.Tenants;
using Nms.Infrastructure.Ticketing;
using Nms.Infrastructure.Ticketing.Jira;
using Nms.Infrastructure.Ticketing.Options;
using Nms.Infrastructure.Ticketing.ServiceNow;
using Nms.Infrastructure.Topology;
using Nms.Infrastructure.Vulnerabilities.Options;
using Nms.Infrastructure.Vulnerabilities.Providers;
using StackExchange.Redis;

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

        // 2. DbContext - Connection-Pooled SQL Server Registration with Transient Resilience
        services.AddDbContext<NmsDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
            options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly(typeof(NmsDbContext).Assembly.GetName().Name);
                        sqlOptions.CommandTimeout(30);
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
        services.Configure<PollingQueueOptions>(configuration.GetSection(PollingQueueOptions.SectionName));
        services.AddSingleton<IPollingQueue>(sp =>
            new PollingQueue(sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<PollingQueueOptions>>()));
        services.AddScoped<IPollScheduler, PollScheduler>();
        services.AddScoped<INetworkInterfaceRepository, NetworkInterfaceRepository>();
        services.AddScoped<IDeviceHealthHistoryRepository, DeviceHealthHistoryRepository>();
        services.AddScoped<IAlertRuleRepository, AlertRuleRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();

        // 7. Connectivity Services & Adapters
        services.AddTransient<IIcmpPingService, IcmpPingService>();
        services.AddTransient<IConnectionAdapter, IcmpConnectionAdapter>();
        services.AddTransient<IConnectionAdapter, SnmpConnectionAdapter>();
        services.AddTransient<IConnectionAdapterFactory, ConnectionAdapterFactory>();
        services.AddSingleton<ISshClientFactory, SshClientFactory>();
        services.AddTransient<IConnectionAdapter, SshConnectionAdapter>();

        // 8. Bind Notification Options & Repositories
        services.Configure<NotificationOptions>(configuration.GetSection(NotificationOptions.SectionName));
        services.AddScoped<INotificationTemplateRepository, NotificationTemplateRepository>();
        services.AddScoped<INotificationLogRepository, NotificationLogRepository>();

        // 9. Shared HttpClient for HTTP Providers
        services.AddScoped(sp => new HttpClient());

        // 10. Provider Implementations
        services.AddScoped<EmailNotificationProvider>();
        services.AddScoped<WebhookNotificationProvider>();
        services.AddScoped<TeamsNotificationProvider>();
        services.AddScoped<SlackNotificationProvider>();

        services.AddScoped<INotificationProvider>(sp => sp.GetRequiredService<EmailNotificationProvider>());
        services.AddScoped<INotificationProvider>(sp => sp.GetRequiredService<WebhookNotificationProvider>());
        services.AddScoped<INotificationProvider>(sp => sp.GetRequiredService<TeamsNotificationProvider>());
        services.AddScoped<INotificationProvider>(sp => sp.GetRequiredService<SlackNotificationProvider>());

        // 11. SMS Gateway Abstraction
        services.AddSingleton<NullSmsProvider>();
        services.AddSingleton<ISmsProvider>(sp => sp.GetRequiredService<NullSmsProvider>());
        services.AddScoped<INotificationProvider>(sp => sp.GetRequiredService<NullSmsProvider>());

        // 12. Device Event Repository
        services.AddScoped<IEventRepository, EventRepository>();

        // 13. Syslog Message Repository
        services.Configure<SyslogOptions>(configuration.GetSection(SyslogOptions.SectionName));
        services.AddSingleton<ISyslogParser, SyslogParser>();
        services.AddSingleton<SyslogUdpReceiver>();
        services.AddSingleton<ISyslogReceiver>(sp => sp.GetRequiredService<SyslogUdpReceiver>());
        services.AddScoped<ISyslogRepository, SyslogRepository>();

        // 14. SNMP Trap Message Repository
        services.Configure<SnmpTrapOptions>(configuration.GetSection(SnmpTrapOptions.SectionName));
        services.AddSingleton<ISnmpTrapParser, SnmpTrapParser>();
        services.AddSingleton<SnmpTrapUdpReceiver>();
        services.AddSingleton<ISnmpTrapReceiver>(sp => sp.GetRequiredService<SnmpTrapUdpReceiver>());
        services.AddScoped<ISnmpTrapRepository, SnmpTrapRepository>();

        // 15. Configuration Backup Module
        services.AddScoped<IConfigurationStorageService, LocalConfigurationStorageService>();
        services.AddScoped<IConfigurationBackupRepository, ConfigurationBackupRepository>();
        services.AddScoped<IBackupScheduleRepository, BackupScheduleRepository>();
        services.AddScoped<IConfigurationBackupEngine, ConfigurationBackupEngine>();
        services.AddScoped<IConfigurationRestoreLogRepository, ConfigurationRestoreLogRepository>();
        services.AddScoped<IConfigurationRestoreEngine, ConfigurationRestoreEngine>();

        // 16. Firmware Upgrade Module
        services.AddScoped<IFirmwareBaselineRepository, FirmwareBaselineRepository>();
        services.AddScoped<IFirmwareUpgradePlanRepository, FirmwareUpgradePlanRepository>();

        // 17. Network Topology Module
        services.AddScoped<ITopologyLinkRepository, TopologyLinkRepository>();
        services.AddScoped<INeighborDiscoveryEngine, NeighborDiscoveryEngine>();


        // 18. Reporting Repositories
        services.AddScoped<IScheduledReportRepository, ScheduledReportRepository>();
        services.AddScoped<IScheduledReportExecutionLogRepository, ScheduledReportExecutionLogRepository>();

        // 19. Audit Logging Repository
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();

        // 20. Asset Management Repository
        services.AddScoped<IAssetRepository, AssetRepository>();

        // 21. Site Management Repository
        services.AddScoped<ISiteRepository, SiteRepository>();

        // 22. Building Management Repository
        services.AddScoped<IBuildingRepository, BuildingRepository>();

        // 23. Floor Management Repository
        services.AddScoped<IFloorRepository, FloorRepository>();

        // 24. Room Management Repository
        services.AddScoped<IRoomRepository, RoomRepository>();

        // 25. Rack Management Repository
        services.AddScoped<IRackRepository, RackRepository>();

        // 26. Customer Management Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ICustomerContactRepository, CustomerContactRepository>();

        // 27.(Ticketing Options, Repositories, Clients & Providers)
        services.Configure<TicketingOptions>(configuration.GetSection(TicketingOptions.SectionName));
        services.Configure<ServiceNowOptions>(configuration.GetSection(ServiceNowOptions.SectionName));
        services.Configure<JiraOptions>(configuration.GetSection(JiraOptions.SectionName));

        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<ITicketSyncLogRepository, TicketSyncLogRepository>();

        services.AddScoped<IServiceNowClient, ServiceNowClient>();
        services.AddScoped<IJiraClient, JiraClient>();

        services.AddScoped<ServiceNowTicketingProvider>();
        services.AddScoped<JiraTicketingProvider>();
        services.AddScoped<ITicketingProviderFactory, TicketingProviderFactory>();

        // 28. Compliance Policy & Device Compliance Scan Repositories
        services.AddScoped<ICompliancePolicyRepository, CompliancePolicyRepository>();
        services.AddScoped<IDeviceComplianceScanRepository, DeviceComplianceScanRepository>();
        services.AddScoped<IDeviceSecurityEvaluationAdapter, DeviceSecurityEvaluationAdapter>();

        // 29. Vulnerability & Firmware Analysis Repositories & Providers
        services.Configure<VulnerabilityIntelligenceOptions>(configuration.GetSection(VulnerabilityIntelligenceOptions.SectionName));
        services.AddScoped<IVulnerabilityRepository, VulnerabilityRepository>();
        services.AddScoped<ISecurityAdvisoryRepository, SecurityAdvisoryRepository>();
        services.AddScoped<IDeviceVulnerabilityMatchRepository, DeviceVulnerabilityMatchRepository>();
        services.AddScoped<IFirmwareUpgradeRecommendationRepository, FirmwareUpgradeRecommendationRepository>();
        services.AddScoped<IVulnerabilityIntelligenceProvider, LocalVulnerabilityIntelligenceProvider>();

        // 30. Threat Intelligence Repositories
        services.AddScoped<IThreatIndicatorRepository, ThreatIndicatorRepository>();
        services.AddScoped<IThreatDetectionRuleRepository, ThreatDetectionRuleRepository>();
        services.AddScoped<IConfigurationDriftRepository, ConfigurationDriftRepository>();


        // 31. Caching Services
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));

        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            try
            {
                var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString, options =>
                {
                    options.AbortOnConnectFail = false;
                    options.ConnectRetry = 2;
                });
                services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            }
            catch
            {
                // Fail-safe registration allows app to run even if Redis is temporarily offline during setup/tests
                services.AddSingleton<IConnectionMultiplexer>(_ => null!);
            }
        }
        else
        {
            services.AddSingleton<IConnectionMultiplexer>(_ => null!);
        }

        services.AddScoped<ICacheService, RedisCacheService>();
        return services;
    }
}