namespace Nms.Domain.Common;

/// <summary>
/// Abstract base class for all domain entities providing strongly-typed identifier baseline.
/// </summary>
public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;

    protected BaseEntity(TId id)
    {
        Id = id;
    }

    // Required for EF Core materialization
    protected BaseEntity() { }

    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (EqualityContract != other.EqualityContract)
            return false;

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(EqualityContract, Id);
    }

    protected virtual Type EqualityContract => GetType();

    public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        return !Equals(left, right);
    }
}