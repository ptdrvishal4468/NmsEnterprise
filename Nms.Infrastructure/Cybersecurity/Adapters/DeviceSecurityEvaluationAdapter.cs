using Microsoft.Extensions.Logging;
using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Infrastructure.Cybersecurity.Adapters;

/// <summary>
/// Infrastructure adapter responsible for interacting with device communication protocols
/// and performing active or passive security configuration evaluation.
/// </summary>
public interface IDeviceSecurityEvaluationAdapter
{
    Task<bool> IsSnmpV3ConfiguredAsync(Device device, CancellationToken cancellationToken = default);
    Task<bool> IsSecureTransportAvailableAsync(Device device, CancellationToken cancellationToken = default);
}

public class DeviceSecurityEvaluationAdapter : IDeviceSecurityEvaluationAdapter
{
    private readonly ILogger<DeviceSecurityEvaluationAdapter> _logger;

    public DeviceSecurityEvaluationAdapter(ILogger<DeviceSecurityEvaluationAdapter> logger)
    {
        _logger = logger;
    }

    public Task<bool> IsSnmpV3ConfiguredAsync(Device device, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        // Verify SNMPv3 credentials existence without logging sensitive data
        bool hasUser = !string.IsNullOrWhiteSpace(device.SnmpV3User);
        bool hasAuth = !string.IsNullOrWhiteSpace(device.SnmpV3AuthKeyEncrypted);
        bool hasPriv = !string.IsNullOrWhiteSpace(device.SnmpV3PrivKeyEncrypted);

        _logger.LogDebug(
            "Evaluated SNMPv3 credential state for device {DeviceId} ({DeviceName}). Configured: {IsConfigured}",
            device.Id,
            device.Name,
            hasUser && hasAuth);

        return Task.FromResult(hasUser && hasAuth);
    }

    public Task<bool> IsSecureTransportAvailableAsync(Device device, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        // Devices with non-standard plain SNMP ports or configured v3 credentials satisfy secure transport baselines
        bool isSecure = !string.IsNullOrWhiteSpace(device.SnmpV3User) || device.SnmpPort != 161;

        _logger.LogDebug(
            "Evaluated transport security posture for device {DeviceId} ({DeviceName}). SecureTransport: {IsSecure}",
            device.Id,
            device.Name,
            isSecure);

        return Task.FromResult(isSecure);
    }
}