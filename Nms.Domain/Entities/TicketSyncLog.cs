using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class TicketSyncLog : BaseEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid TicketId { get; set; }
    public TicketingProviderType ProviderType { get; set; }
    public TicketSyncDirection Direction { get; set; }
    public TicketSyncStatus Status { get; set; }
    public string? RequestPayload { get; set; }
    public string? ResponsePayload { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public Ticket? Ticket { get; set; }

    public TicketSyncLog() : base(Guid.NewGuid())
    {
    }

    public TicketSyncLog(Guid id) : base(id)
    {
    }
}