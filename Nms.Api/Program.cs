using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Nms.Api.HostedServices;
using Nms.Api.Middleware;
using Nms.Application;
using Nms.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Forwarded Headers configuration for reverse proxy TLS offloading
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto |
                               ForwardedHeaders.XForwardedHost;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

// Bootstrap Application & Infrastructure Layers
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);

// Register Hosted Services
builder.Services.AddHostedService<SnmpPollingBackgroundService>();
builder.Services.AddHostedService<IcmpPollingBackgroundService>();
builder.Services.AddHostedService<SyslogListenerBackgroundService>();
builder.Services.AddHostedService<SnmpTrapListenerBackgroundService>();
builder.Services.AddHostedService<BackupSchedulerBackgroundService>();
builder.Services.AddHostedService<ReportSchedulerBackgroundService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 1. Forwarded headers MUST execute at the very start so downstream middleware sees correct scheme/IP
app.UseForwardedHeaders();

// 2. Register Global Exception Handling Middleware AT THE VERY TOP of the pipeline
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

// 3. Authentication MUST execute first to extract User Claims from JWT
app.UseAuthentication();

// 4. TenantResolverMiddleware MUST execute after Authentication so it can extract tenant_id claims
app.UseMiddleware<TenantResolverMiddleware>();

// 5. Authorization executes after TenantContext is established
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

await app.RunAsync();

public partial class Program { }