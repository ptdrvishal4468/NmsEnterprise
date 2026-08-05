using Nms.Domain.Enums;
using Nms.Domain.Models;

namespace Nms.Application.Common.Interfaces;

public interface IConnectionAdapter
{
    NetworkProtocol Protocol { get; }
    Task<ConnectionResult> TestConnectionAsync(ConnectionParameters parameters, CancellationToken cancellationToken);
}