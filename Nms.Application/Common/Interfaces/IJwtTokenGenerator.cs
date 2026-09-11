using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateAccessToken(User user, IEnumerable<string> permissions);
    string GenerateRefreshToken();
}