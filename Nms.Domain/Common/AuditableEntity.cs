namespace Nms.Domain.Common;

/// <summary>
/// Base class for entities requiring creation and modification audit timestamps.
/// </summary>
public abstract class AuditableEntity<TId> : BaseEntity<TId>
{
    public DateTime CreatedAtUtc { get; protected set; } = DateTime.UtcNow;
    public string? CreatedBy { get; protected set; }
    public DateTime? LastModifiedAtUtc { get; protected set; }
    public string? LastModifiedBy { get; protected set; }

    protected AuditableEntity(TId id) : base(id) { }
    protected AuditableEntity() { }

    public void SetCreatedAudit(string createdBy)
    {
        CreatedAtUtc = DateTime.UtcNow;
        CreatedBy = createdBy;
    }

    public void SetModifiedAudit(string modifiedBy)
    {
        LastModifiedAtUtc = DateTime.UtcNow;
        LastModifiedBy = modifiedBy;
    }
}