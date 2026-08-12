namespace Nms.Application.Common.Interfaces;

public interface ISmsProvider
{
    Task<bool> SendSmsAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default);
}