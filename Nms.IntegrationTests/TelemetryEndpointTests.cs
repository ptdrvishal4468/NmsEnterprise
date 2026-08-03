using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nms.Application.Telemetry.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Infrastructure.Data;
using Nms.Infrastructure.Data.Interceptors;
using Nms.Infrastructure.Security;
using Xunit;

namespace Nms.IntegrationTests;

public class TelemetryEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public TelemetryEndpointTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDeviceMetrics_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<NmsDbContext>();

            var device = (Device)Activator.CreateInstance(typeof(Device), true)!;

            var entry = db.Entry(device);
            entry.Property("Id").CurrentValue = deviceId;
            entry.Property("TenantId").CurrentValue = tenantId;
            entry.Property("Name").CurrentValue = "Core Switch 01";
            entry.Property("IpAddress").CurrentValue = "192.168.1.1";
            entry.Property("DeviceType").CurrentValue = DeviceType.Switch;
            entry.Property("Status").CurrentValue = DeviceStatus.Online;
            entry.Property("SnmpPort").CurrentValue = 161;

            db.Devices.Add(device);
            await db.SaveChangesAsync();
        }

        var fromUtc = DateTime.UtcNow.AddHours(-1).ToString("o");
        var toUtc = DateTime.UtcNow.ToString("o");

        // Act
        var response = await _client.GetAsync($"/api/v1/telemetry/devices/{deviceId}/metrics?fromUtc={Uri.EscapeDataString(fromUtc)}&toUtc={Uri.EscapeDataString(toUtc)}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var metrics = await response.Content.ReadFromJsonAsync<IEnumerable<DeviceMetricDto>>();
        Assert.NotNull(metrics);
    }
}

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string _dbName = "TelemetryTestsDb";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<NmsDbContext>>();
            services.RemoveAll<DbContextOptions>();
            services.RemoveAll<IDbContextOptionsConfiguration<NmsDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<DbContext>>();
            services.RemoveAll<NmsDbContext>();
            services.RemoveAll<AuditableEntityInterceptor>();
            services.RemoveAll<IAuthorizationHandler>();

            services.AddScoped<AuditableEntityInterceptor>();
            services.AddScoped<IAuthorizationHandler, TestPermissionAuthorizationHandler>();
            services.AddAuthentication("TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", _ => { });
            services.AddAuthorization();

            services.AddDbContext<NmsDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
                options.UseInMemoryDatabase(_dbName)
                       .AddInterceptors(interceptor);
            }, ServiceLifetime.Scoped, ServiceLifetime.Scoped);
        });
    }
}

internal sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "integration-test-user")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

internal sealed class TestPermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}