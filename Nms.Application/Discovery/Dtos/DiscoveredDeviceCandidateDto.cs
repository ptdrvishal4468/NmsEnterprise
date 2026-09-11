using Nms.Domain.Enums;

namespace Nms.Application.Discovery.Dtos;

public record DiscoveredDeviceCandidateDto(
    Guid Id,
    Guid DiscoveryJobId,
    string IpAddress,
    bool IsIcmpReachable,
    double? ResponseTimeMs,
    bool IsSnmpReachable,
    string? SysDescr,
    string? SysObjectId,
    string? SysName,
    DeviceType FingerprintedType,
    string IdentifiedVendor,
    bool IsDuplicate,
    Guid? ExistingDeviceId,
    bool IsImported,
    Guid? ImportedDeviceId);