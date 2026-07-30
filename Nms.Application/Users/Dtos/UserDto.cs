namespace Nms.Application.Users.Dtos;

public record UserDto(
    Guid Id,
    Guid TenantId,
    string Email,
    bool IsActive,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<string> Roles
);