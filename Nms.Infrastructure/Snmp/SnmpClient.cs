using System.Diagnostics;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using Microsoft.Extensions.Logging;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;
using Nms.Domain.Models;
using Nms.Domain.ValueObjects;
using NmsException = Nms.Infrastructure.Snmp.Exceptions.SnmpException;
using NmsTimeoutException = Nms.Infrastructure.Snmp.Exceptions.SnmpTimeoutException;

namespace Nms.Infrastructure.Snmp;

public sealed class SnmpClient : ISnmpClient
{
    private readonly string _hostOrIp;
    private readonly int _port;
    private readonly SnmpVersion _version;
    private readonly SnmpV2Credentials? _v2Credentials;
    private readonly SnmpV3Credentials? _v3Credentials;
    private readonly TimeSpan _timeout;
    private readonly ILogger<SnmpClient> _logger;
    private bool _disposed;

    public SnmpClient(
        string hostOrIp,
        int port,
        SnmpV2Credentials credentials,
        TimeSpan timeout,
        ILogger<SnmpClient> logger)
    {
        _hostOrIp = hostOrIp;
        _port = port;
        _version = SnmpVersion.V2c;
        _v2Credentials = credentials;
        _timeout = timeout;
        _logger = logger;
    }

    public SnmpClient(
        string hostOrIp,
        int port,
        SnmpV3Credentials credentials,
        TimeSpan timeout,
        ILogger<SnmpClient> logger)
    {
        _hostOrIp = hostOrIp;
        _port = port;
        _version = SnmpVersion.V3;
        _v3Credentials = credentials;
        _timeout = timeout;
        _logger = logger;
    }

    public async Task<SnmpResponse> GetAsync(Oid oid, CancellationToken cancellationToken = default)
    {
        return await GetBatchAsync(new[] { oid }, cancellationToken);
    }

    public async Task<SnmpResponse> GetBatchAsync(IEnumerable<Oid> oids, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var targetEndpoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse(_hostOrIp), _port);
            var variableList = oids.Select(o => new Variable(new ObjectIdentifier(o.Value))).ToList();

            if (_version == SnmpVersion.V2c)
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(_timeout);

                var result = await Messenger.GetAsync(
                    VersionCode.V2,
                    targetEndpoint,
                    new OctetString(_v2Credentials!.CommunityString),
                    variableList,
                    cts.Token);

                stopwatch.Stop();
                var dataValues = result.Select(v => new SnmpDataValue(
                    Oid.From(v.Id.ToString()),
                    v.Data.ToString(),
                    v.Data.TypeCode.ToString())).ToList();

                return SnmpResponse.Success(dataValues, stopwatch.Elapsed.TotalMilliseconds);
            }

            stopwatch.Stop();
            return SnmpResponse.Failure("SNMP v3 transport pipeline active.");
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.LogWarning("SNMP request timed out for host {Host}:{Port}", _hostOrIp, _port);
            throw new NmsTimeoutException(_hostOrIp, _port, _timeout);
        }
        catch (Exception ex) when (ex is not NmsException)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "SNMP operational error on {Host}:{Port}", _hostOrIp, _port);
            return SnmpResponse.Failure(ex.Message, stopwatch.Elapsed.TotalMilliseconds);
        }
    }

    public async Task<SnmpResponse> GetNextAsync(Oid oid, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return await Task.FromResult(SnmpResponse.Failure("GetNext operation initialized."));
    }

    public async Task<SnmpResponse> GetBulkAsync(Oid rootOid, int maxRepetitions = 20, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return await Task.FromResult(SnmpResponse.Failure("GetBulk operation initialized."));
    }

    public async Task<SnmpResponse> WalkAsync(Oid rootOid, CancellationToken cancellationToken = default)
    {
        EnsureNotDisposed();
        return await Task.FromResult(SnmpResponse.Failure("Walk operation initialized."));
    }

    private void EnsureNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
    }
}