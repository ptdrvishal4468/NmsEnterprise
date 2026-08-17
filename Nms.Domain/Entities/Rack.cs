namespace Nms.Domain.Entities;

using Nms.Domain.Common;

public class Rack : AuditableEntity<Guid>, IMustHaveTenant
{
    public Guid TenantId { get; set; }
    public Guid RoomId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Identifier { get; private set; } = string.Empty;
    public int HeightInUnits { get; private set; } = 42;
    public int? WidthInInches { get; private set; } = 19;
    public int? DepthInMm { get; private set; }
    public decimal? MaxPowerWatts { get; private set; }
    public decimal? MaxWeightKg { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigations
    public Room Room { get; private set; } = null!;

    protected Rack() : base() { }

    public Rack(
        Guid id,
        Guid tenantId,
        Guid roomId,
        string name,
        string identifier,
        int heightInUnits = 42,
        int? widthInInches = 19,
        int? depthInMm = null,
        decimal? maxPowerWatts = null,
        decimal? maxWeightKg = null,
        string? notes = null) : base(id)
    {
        TenantId = tenantId;
        RoomId = roomId;
        Update(name, identifier, heightInUnits, widthInInches, depthInMm, maxPowerWatts, maxWeightKg, notes);
    }

    public void Update(
        string name,
        string identifier,
        int heightInUnits,
        int? widthInInches,
        int? depthInMm,
        decimal? maxPowerWatts,
        decimal? maxWeightKg,
        string? notes)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(identifier);

        if (heightInUnits <= 0)
            throw new ArgumentOutOfRangeException(nameof(heightInUnits), "Rack height in units must be greater than zero.");

        Name = name.Trim();
        Identifier = identifier.Trim().ToUpperInvariant();
        HeightInUnits = heightInUnits;
        WidthInInches = widthInInches;
        DepthInMm = depthInMm;
        MaxPowerWatts = maxPowerWatts;
        MaxWeightKg = maxWeightKg;
        Notes = notes?.Trim();
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}