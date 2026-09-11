using Nms.Domain.Models;
using Nms.Domain.ValueObjects;

namespace Nms.Application.Common.Interfaces;

public interface ISnmpClient : IDisposable
{
    Task<SnmpResponse> GetAsync(Oid oid, CancellationToken cancellationToken = default);
    Task<SnmpResponse> GetBatchAsync(IEnumerable<Oid> oids, CancellationToken cancellationToken = default);
    Task<SnmpResponse> GetNextAsync(Oid oid, CancellationToken cancellationToken = default);
    Task<SnmpResponse> GetBulkAsync(Oid rootOid, int maxRepetitions = 20, CancellationToken cancellationToken = default);
    Task<SnmpResponse> WalkAsync(Oid rootOid, CancellationToken cancellationToken = default);
}