using Nms.Domain.Common;

namespace Nms.Domain.Entities;

public class PollProfile : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int IntervalSeconds { get; private set; } = 60;
    public int TimeoutSeconds { get; private set; } = 5;
    public int RetryCount { get; private set; } = 3;
    public bool IsDefault { get; private set; }
    public bool IsEnabled { get; private set; } = true;

    // Navigation property
    private readonly List<Device> _devices = new();
    public IReadOnlyCollection<Device> Devices => _devices.AsReadOnly();

    // EF Core private constructor
    private PollProfile() { }

    public PollProfile(
        Guid id,
        Guid tenantId,
        string name,
        int intervalSeconds = 60,
        int timeoutSeconds = 5,
        int retryCount = 3,
        bool isDefault = false,
        string? description = null) : base(id)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Poll profile name is required.", nameof(name));

        if (intervalSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(intervalSeconds), "Interval must be greater than 0 seconds.");

        if (timeoutSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeoutSeconds), "Timeout must be greater than 0 seconds.");

        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount), "Retry count cannot be negative.");

        TenantId = tenantId;
        Name = name;
        IntervalSeconds = intervalSeconds;
        TimeoutSeconds = timeoutSeconds;
        RetryCount = retryCount;
        IsDefault = isDefault;
        Description = description;
        IsEnabled = true;
    }

    public void UpdateProfile(
        string name,
        int intervalSeconds,
        int timeoutSeconds,
        int retryCount,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Poll profile name is required.", nameof(name));

        if (intervalSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(intervalSeconds), "Interval must be greater than 0 seconds.");

        if (timeoutSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(timeoutSeconds), "Timeout must be greater than 0 seconds.");

        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount), "Retry count cannot be negative.");

        Name = name;
        IntervalSeconds = intervalSeconds;
        TimeoutSeconds = timeoutSeconds;
        RetryCount = retryCount;
        Description = description;
    }

    public void SetDefault(bool isDefault)
    {
        IsDefault = isDefault;
    }

    public void SetStatus(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}