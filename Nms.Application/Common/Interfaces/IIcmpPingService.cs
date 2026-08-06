using Nms.Domain.Models;

namespace Nms.Application.Common.Interfaces;

public interface IIcmpPingService
{
    Task<PingResult> PingAsync(string hostOrIp, int count = 4, int timeoutMs = 1000, CancellationToken cancellationToken = default);
}