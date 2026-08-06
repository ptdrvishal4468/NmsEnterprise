using Microsoft.Extensions.Logging;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;
using Nms.Domain.Models;
using Nms.Domain.ValueObjects;

namespace Nms.Infrastructure.Snmp;

public sealed class SnmpClientFactory : ISnmpClientFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public SnmpClientFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    public ISnmpClient CreateV2cClient(string hostOrIp, int port, SnmpV2Credentials credentials, TimeSpan? timeout = null)
    {
        var logger = _loggerFactory.CreateLogger<SnmpClient>();
        return new SnmpClient(hostOrIp, port, credentials, timeout ?? TimeSpan.FromSeconds(3), logger);
    }

    public ISnmpClient CreateV3Client(string hostOrIp, int port, SnmpV3Credentials credentials, TimeSpan? timeout = null)
    {
        var logger = _loggerFactory.CreateLogger<SnmpClient>();
        return new SnmpClient(hostOrIp, port, credentials, timeout ?? TimeSpan.FromSeconds(3), logger);
    }

    public ISnmpClient CreateClient(ConnectionParameters connectionParameters)
    {
        ArgumentNullException.ThrowIfNull(connectionParameters);

        if (connectionParameters.Protocol == NetworkProtocol.SnmpV2c)
        {
            var creds = new SnmpV2Credentials(connectionParameters.CommunityString ?? "public");
            return CreateV2cClient(connectionParameters.HostOrIp, connectionParameters.Port, creds, connectionParameters.Timeout);
        }

        throw new NotSupportedException($"Protocol {connectionParameters.Protocol} is not directly supported by SnmpClientFactory.");
    }
}