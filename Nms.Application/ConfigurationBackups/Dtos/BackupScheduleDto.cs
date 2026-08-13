namespace Nms.Application.ConfigurationBackups.Dtos;

public record BackupScheduleDto
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }

    public Guid? DeviceId { get; init; }
    public string? DeviceName { get; init; }

    public int IntervalMinutes { get; init; }
    public bool IsEnabled { get; init; }

    public DateTime? LastRunUtc { get; init; }
    public DateTime NextRunUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}