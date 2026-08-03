using Nms.Api.HostedServices;
using Nms.Api.Middleware;
using Nms.Application;
using Nms.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Bootstrap Application & Infrastructure Layers
builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureLayer(builder.Configuration);

// Register SNMP Polling Hosted Service
builder.Services.AddHostedService<SnmpPollingBackgroundService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseHttpsRedirection();
}

// 1. Authentication MUST execute first to extract User Claims from JWT
app.UseAuthentication();

// 2. TenantResolverMiddleware MUST execute after Authentication so it can extract tenant_id claims
app.UseMiddleware<TenantResolverMiddleware>();

// 3. Authorization executes after TenantContext is established
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program { }