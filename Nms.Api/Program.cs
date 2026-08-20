using Nms.Api.HostedServices;
using Nms.Api.Middleware;
using Nms.Application;
using Nms.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

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

// 1. Register Global Exception Handling Middleware AT THE VERY TOP of the pipeline
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

// 2. Authentication MUST execute first to extract User Claims from JWT
app.UseAuthentication();

// 3. TenantResolverMiddleware MUST execute after Authentication so it can extract tenant_id claims
app.UseMiddleware<TenantResolverMiddleware>();

// 4. Authorization executes after TenantContext is established
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