namespace Nms.Application.Sites.Dtos;

public record CreateSiteDto(
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
    string? TimeZone);