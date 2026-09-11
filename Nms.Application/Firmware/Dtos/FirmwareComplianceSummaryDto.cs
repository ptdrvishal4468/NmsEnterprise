namespace Nms.Application.Firmware.Dtos;

public record FirmwareComplianceSummaryDto(
    int TotalDevices,
    int CompliantDevices,
    int NonCompliantDevices,
    int UnknownDevices,
    double CompliancePercentage);