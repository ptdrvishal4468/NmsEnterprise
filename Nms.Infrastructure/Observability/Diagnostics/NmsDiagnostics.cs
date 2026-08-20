using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Nms.Infrastructure.Observability.Diagnostics;

public static class NmsDiagnostics
{
    public const string ServiceName = "NmsEnterprise";
    public const string ServiceVersion = "1.0.0";

    public static readonly ActivitySource ActivitySource = new(ServiceName, ServiceVersion);
    public static readonly ActivitySource PollingActivitySource = new("NmsEnterprise.Polling", ServiceVersion);
    public static readonly ActivitySource AlertsActivitySource = new("NmsEnterprise.Alerts", ServiceVersion);
    public static readonly ActivitySource DiscoveryActivitySource = new("NmsEnterprise.Discovery", ServiceVersion);
    public static readonly ActivitySource ConnectivityActivitySource = new("NmsEnterprise.Connectivity", ServiceVersion);

    public static readonly Meter Meter = new(ServiceName, ServiceVersion);
}