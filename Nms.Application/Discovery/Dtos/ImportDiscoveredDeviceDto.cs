namespace Nms.Application.Discovery.Dtos;

public record ImportDiscoveredDeviceDto(
    Guid CandidateId,
    string DeviceName);