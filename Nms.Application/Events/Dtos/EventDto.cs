using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Application.Events.Dtos;

public class EventDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid? DeviceId { get; set; }
    public EventCategory Category { get; set; }
    public EventSeverity Severity { get; set; }
    public string Source { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? CorrelationId { get; set; }
    public Guid? ParentEventId { get; set; }
    public string? MetadataJson { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }

    public static EventDto FromEntity(DeviceEvent entity)
    {
        return new EventDto
        {
            Id = entity.Id,
            TenantId = entity.TenantId,
            DeviceId = entity.DeviceId,
            Category = entity.Category,
            Severity = entity.Severity,
            Source = entity.Source,
            Message = entity.Message,
            CorrelationId = entity.CorrelationId,
            ParentEventId = entity.ParentEventId,
            MetadataJson = entity.MetadataJson,
            CreatedAtUtc = entity.CreatedAtUtc,
            CreatedBy = entity.CreatedBy
        };
    }
}