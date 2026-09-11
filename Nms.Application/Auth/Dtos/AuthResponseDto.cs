namespace Nms.Application.Auth.Dtos;

public record AuthResponseDto(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt
);