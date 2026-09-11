namespace Nms.Application.Sites.Dtos;

public record UpdateSiteDto(
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
    bool IsActive);