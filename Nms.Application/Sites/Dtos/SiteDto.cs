namespace Nms.Application.Sites.Dtos;

public record SiteDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string Code,
    string? Description,
    string? Address,
    string? City,
    string? StateOrProvince,
    string? PostalCode,
    string? Country,
    double? Latitude,
    double? Longitude,
    string? TimeZone,
    bool IsActive,
    DateTime CreatedAtUtc,
    string? CreatedBy,
    DateTime? LastModifiedAtUtc,
    string? LastModifiedBy,
    int BuildingCount);