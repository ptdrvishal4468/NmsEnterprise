using Nms.Application.Auth.Dtos;
using Nms.Application.Common.Interfaces;

namespace Nms.Application.Auth.Commands.Login;

public class LoginCommandHandler
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public Task<AuthResponseDto> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        // In Phase 15 (EF Core Data Access), this handler will query IUserRepository,
        // verify password hashes, pull flattened user permissions, and save RefreshTokens.
        throw new NotImplementedException("Handler pipeline ready for EF Core DbContext wiring in Phase 15.");
    }
}