using Microsoft.Extensions.Logging;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.ValueObjects;

namespace Nms.Application.Discovery.Services;

public class DeviceDiscoveryEngine
{
    private readonly IIcmpPingService _icmpPingService;
    private readonly ISnmpClientFactory _snmpClientFactory;
    private readonly DuplicateDetector _duplicateDetector;
    private readonly ILogger<DeviceDiscoveryEngine> _logger;

    private static readonly Oid SysDescrOid = Oid.From("1.3.6.1.2.1.1.1.0");
    private static readonly Oid SysObjectIdOid = Oid.From("1.3.6.1.2.1.1.2.0");
    private static readonly Oid SysNameOid = Oid.From("1.3.6.1.2.1.1.5.0");

    public DeviceDiscoveryEngine(
        IIcmpPingService icmpPingService,
        ISnmpClientFactory snmpClientFactory,
        DuplicateDetector duplicateDetector,
        ILogger<DeviceDiscoveryEngine> logger)
    {
        _icmpPingService = icmpPingService;
        _snmpClientFactory = snmpClientFactory;
        _duplicateDetector = duplicateDetector;
        _logger = logger;
    }

    public async Task ExecuteScanAsync(
        DiscoveryJob job,
        int maxConcurrency = 20,
        CancellationToken cancellationToken = default)
    {
        var targetIps = IpRangeCalculator.CalculateIpAddresses(job.IpRange);
        job.Start(targetIps.Count);

        using var semaphore = new SemaphoreSlim(maxConcurrency);
        int processedCount = 0;
        int discoveredCount = 0;

        var tasks = targetIps.Select(async ipAddress =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                if (cancellationToken.IsCancellationRequested) return;

                var candidate = await ProbeTargetAsync(job, ipAddress, cancellationToken);
                if (candidate != null)
                {
                    lock (job)
                    {
                        job.AddCandidate(candidate);
                        Interlocked.Increment(ref discoveredCount);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error scanning target {IpAddress} for Job {JobId}", ipAddress, job.Id);
            }
            finally
            {
                int currentProcessed = Interlocked.Increment(ref processedCount);
                job.Progress(currentProcessed, discoveredCount);
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        if (cancellationToken.IsCancellationRequested)
        {
            job.Cancel();
        }
        else
        {
            job.Complete();
        }
    }

    private async Task<DiscoveredDeviceCandidate?> ProbeTargetAsync(
        DiscoveryJob job,
        string ipAddress,
        CancellationToken cancellationToken)
    {
        var pingResult = await _icmpPingService.PingAsync(ipAddress, count: 2, timeoutMs: 1000, cancellationToken);
        bool isIcmpReachable = pingResult.Status == ReachabilityStatus.Online || pingResult.PacketsReceived > 0;

        bool isSnmpReachable = false;
        string? sysDescr = null;
        string? sysObjectId = null;
        string? sysName = null;

        if (!string.IsNullOrWhiteSpace(job.SnmpCommunity))
        {
            try
            {
                var creds = new SnmpV2Credentials(job.SnmpCommunity);
                using var snmpClient = _snmpClientFactory.CreateV2cClient(ipAddress, job.SnmpPort, creds, TimeSpan.FromSeconds(2));

                var descrResponse = await snmpClient.GetAsync(SysDescrOid, cancellationToken);
                if (descrResponse.IsSuccess && descrResponse.DataValues.Count > 0)
                {
                    sysDescr = descrResponse.DataValues[0].RawValue;
                    isSnmpReachable = true;
                }

                var objIdResponse = await snmpClient.GetAsync(SysObjectIdOid, cancellationToken);
                if (objIdResponse.IsSuccess && objIdResponse.DataValues.Count > 0)
                {
                    sysObjectId = objIdResponse.DataValues[0].RawValue;
                }

                var nameResponse = await snmpClient.GetAsync(SysNameOid, cancellationToken);
                if (nameResponse.IsSuccess && nameResponse.DataValues.Count > 0)
                {
                    sysName = nameResponse.DataValues[0].RawValue;
                }
            }
            catch
            {
                // SNMP probe failure does not abort ICMP candidate detection
            }
        }

        if (!isIcmpReachable && !isSnmpReachable)
        {
            return null;
        }

        var fingerprintedType = DeviceFingerprinter.FingerprintDevice(sysObjectId, sysDescr);
        var identifiedVendor = VendorIdentifier.IdentifyVendor(sysObjectId, sysDescr);
        var (isDuplicate, existingDeviceId) = await _duplicateDetector.DetectAsync(ipAddress, cancellationToken);

        return new DiscoveredDeviceCandidate(
            id: Guid.NewGuid(),
            tenantId: job.TenantId,
            discoveryJobId: job.Id,
            ipAddress: ipAddress,
            isIcmpReachable: isIcmpReachable,
            responseTimeMs: isIcmpReachable ? pingResult.AvgLatencyMs : null,
            isSnmpReachable: isSnmpReachable,
            sysDescr: sysDescr,
            sysObjectId: sysObjectId,
            sysName: sysName,
            macAddress: null,
            fingerprintedType: fingerprintedType,
            identifiedVendor: identifiedVendor,
            isDuplicate: isDuplicate,
            existingDeviceId: existingDeviceId);
    }
}