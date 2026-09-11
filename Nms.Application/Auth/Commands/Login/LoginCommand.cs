using Nms.Application.Auth.Dtos;

namespace Nms.Application.Auth.Commands.Login;

public record LoginCommand(
    string Email,
    string Password
);