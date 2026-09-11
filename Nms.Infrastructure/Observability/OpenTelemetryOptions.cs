namespace Nms.Infrastructure.Observability;

public sealed class OpenTelemetryOptions
{
    public const string SectionName = "OpenTelemetry";

    public string ServiceName { get; set; } = "NmsEnterprise.Api";
    public string ServiceVersion { get; set; } = "1.0.0";
    public string? OtlpEndpoint { get; set; } = "http://localhost:4317";
    public bool EnableConsoleExporter { get; set; }
    public bool EnableOtlpExporter { get; set; }
    public bool EnableTracing { get; set; } = true;
    public bool EnableMetrics { get; set; } = true;
}