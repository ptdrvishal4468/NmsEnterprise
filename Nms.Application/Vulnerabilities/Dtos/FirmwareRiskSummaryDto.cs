namespace Nms.Application.Vulnerabilities.Dtos;

public record FirmwareRiskSummaryDto(
    int TotalDevicesScanned,
    int TotalVulnerableDevices,
    int TotalActiveVulnerabilities,
    int CriticalVulnerabilities,
    int HighVulnerabilities,
    int MediumVulnerabilities,
    int LowVulnerabilities,
    int TotalPendingUpgradeRecommendations);