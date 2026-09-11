namespace Nms.Application.Users.Dtos;

public record CreateUserResponseDto(
    Guid UserId,
    string Email,
    Guid TenantId,
    DateTime CreatedAtUtc
);