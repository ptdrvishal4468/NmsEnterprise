using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nms.Application.Telemetry.Dtos;
using Nms.Infrastructure.Data;
using Nms.Infrastructure.Data.Interceptors;
using Xunit;

namespace Nms.IntegrationTests;

public class TelemetryEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TelemetryEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDeviceMetrics_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var fromUtc = DateTime.UtcNow.AddHours(-1).ToString("o");
        var toUtc = DateTime.UtcNow.ToString("o");

        // Act
        var response = await _client.GetAsync($"/api/telemetry/devices/{deviceId}/metrics?fromUtc={Uri.EscapeDataString(fromUtc)}&toUtc={Uri.EscapeDataString(toUtc)}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var metrics = await response.Content.ReadFromJsonAsync<IEnumerable<DeviceMetricDto>>();
        Assert.NotNull(metrics);
    }
}

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
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

            services.AddScoped<AuditableEntityInterceptor>();
            services.AddDbContext<NmsDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<AuditableEntityInterceptor>();
                options.UseInMemoryDatabase("TelemetryTestsDb")
                    .AddInterceptors(interceptor);
            });
        });
    }
}