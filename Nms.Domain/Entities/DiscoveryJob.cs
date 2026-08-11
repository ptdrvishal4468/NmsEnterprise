using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class DiscoveryJob : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string IpRange { get; private set; } = string.Empty;
    public string? SnmpCommunity { get; private set; }
    public int SnmpPort { get; private set; } = 161;
    public DiscoveryJobStatus Status { get; private set; } = DiscoveryJobStatus.Pending;
    public int TotalTargets { get; private set; }
    public int ProcessedTargets { get; private set; }
    public int DiscoveredCount { get; private set; }
    public DateTime? StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public string? FailureReason { get; private set; }

    private readonly List<DiscoveredDeviceCandidate> _candidates = new();
    public IReadOnlyCollection<DiscoveredDeviceCandidate> Candidates => _candidates.AsReadOnly();

    // Private constructor for EF Core
    private DiscoveryJob() { }

    public DiscoveryJob(
        Guid id,
        Guid tenantId,
        string name,
        string ipRange,
        string? snmpCommunity = "public",
        int snmpPort = 161) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Job name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(ipRange))
            throw new ArgumentException("IP range is required.", nameof(ipRange));

        TenantId = tenantId;
        Name = name;
        IpRange = ipRange;
        SnmpCommunity = snmpCommunity;
        SnmpPort = snmpPort;
        Status = DiscoveryJobStatus.Pending;
    }

    public void Start(int totalTargets)
    {
        Status = DiscoveryJobStatus.Running;
        TotalTargets = totalTargets;
        StartedAtUtc = DateTime.UtcNow;
    }

    public void Progress(int processed, int discovered)
    {
        ProcessedTargets = processed;
        DiscoveredCount = discovered;
    }

    public void Complete()
    {
        Status = DiscoveryJobStatus.Completed;
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void Fail(string reason)
    {
        Status = DiscoveryJobStatus.Failed;
        FailureReason = reason;
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = DiscoveryJobStatus.Cancelled;
        CompletedAtUtc = DateTime.UtcNow;
    }

    public void AddCandidate(DiscoveredDeviceCandidate candidate)
    {
        _candidates.Add(candidate);
    }
}