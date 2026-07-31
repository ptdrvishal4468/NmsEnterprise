using Nms.Application.Common.Models;
using Nms.Domain.Entities;

namespace Nms.Application.Common.Interfaces;

public interface ISnmpCollectorService
{
    Task<SnmpPollResult> PollDeviceAsync(Device device, IEnumerable<string> oids, CancellationToken cancellationToken = default);
    Task<bool> TestConnectionAsync(Device device, CancellationToken cancellationToken = default);
}