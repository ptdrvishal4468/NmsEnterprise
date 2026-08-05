using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;

namespace Nms.Infrastructure.Connectivity;

public sealed class ConnectionAdapterFactory : IConnectionAdapterFactory
{
    private readonly IDictionary<NetworkProtocol, IConnectionAdapter> _adapters;

    public ConnectionAdapterFactory(IEnumerable<IConnectionAdapter> adapters)
    {
        if (adapters == null) throw new ArgumentNullException(nameof(adapters));

        _adapters = adapters.ToDictionary(a => a.Protocol, a => a);
    }

    public IConnectionAdapter GetAdapter(NetworkProtocol protocol)
    {
        if (_adapters.TryGetValue(protocol, out var adapter))
        {
            return adapter;
        }

        throw new NotSupportedException($"No connectivity adapter registered for protocol: {protocol}");
    }
}