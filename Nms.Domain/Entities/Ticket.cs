using Nms.Domain.Common;
using Nms.Domain.Enums;

namespace Nms.Domain.Entities;

public class Ticket : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public TicketStatus Status { get; set; } = TicketStatus.Open;
    public TicketingProviderType ProviderType { get; set; }
    public string? ExternalTicketId { get; set; }
    public string? ExternalTicketKey { get; set; }
    public string? ExternalStatus { get; set; }
    public string? ExternalUrl { get; set; }
    public DateTime? LastSyncedAtUtc { get; set; }
    public Guid? DeviceId { get; set; }
    public Guid? AlertId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? MetadataJson { get; set; }

    public Device? Device { get; set; }
    public Alert? Alert { get; set; }
    public Customer? Customer { get; set; }
    public ICollection<TicketSyncLog> SyncLogs { get; set; } = new List<TicketSyncLog>();

    public Ticket() : base(Guid.NewGuid())
    {
    }

    public Ticket(Guid id) : base(id)
    {
    }

    public void UpdateStatus(TicketStatus newStatus)
    {
        Status = newStatus;
    }

    public void SetExternalDetails(string externalId, string externalKey, string? externalStatus, string? externalUrl)
    {
        ExternalTicketId = externalId;
        ExternalTicketKey = externalKey;
        ExternalStatus = externalStatus;
        ExternalUrl = externalUrl;
        LastSyncedAtUtc = DateTime.UtcNow;
    }
}