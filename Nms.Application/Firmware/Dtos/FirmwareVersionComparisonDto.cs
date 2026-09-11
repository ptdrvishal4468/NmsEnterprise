using Nms.Domain.Enums;

namespace Nms.Application.Firmware.Dtos;

public record FirmwareVersionComparisonDto(
    string Version1,
    string Version2,
    int ComparisonResult,
    string RelationDescription,
    FirmwareComplianceStatus ComplianceStatus);