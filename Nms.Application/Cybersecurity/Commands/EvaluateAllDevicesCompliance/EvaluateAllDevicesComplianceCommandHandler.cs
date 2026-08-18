using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Cybersecurity.Commands.EvaluateAllDevicesCompliance;

public class EvaluateAllDevicesComplianceCommandHandler : IRequestHandler<EvaluateAllDevicesComplianceCommand, IReadOnlyList<DeviceComplianceScanDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICybersecurityComplianceEngine _complianceEngine;

    public EvaluateAllDevicesComplianceCommandHandler(
        IUnitOfWork unitOfWork,
        ICybersecurityComplianceEngine complianceEngine)
    {
        _unitOfWork = unitOfWork;
        _complianceEngine = complianceEngine;
    }

    public async Task<IReadOnlyList<DeviceComplianceScanDto>> Handle(EvaluateAllDevicesComplianceCommand request, CancellationToken cancellationToken)
    {
        var devices = await _unitOfWork.Devices.GetAllAsync(cancellationToken);
        var activePolicies = await _unitOfWork.CompliancePolicies.GetActivePoliciesAsync(cancellationToken);

        var scans = new List<DeviceComplianceScanDto>();

        foreach (var device in devices)
        {
            var applicablePolicies = activePolicies
                .Where(p => (!p.TargetDeviceType.HasValue || p.TargetDeviceType.Value == device.DeviceType) &&
                            (string.IsNullOrWhiteSpace(p.TargetVendor) || string.Equals(p.TargetVendor, device.Vendor, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var scan = await _complianceEngine.EvaluateDeviceAsync(
                device,
                applicablePolicies,
                request.EvaluationNotes,
                cancellationToken);

            await _unitOfWork.DeviceComplianceScans.AddAsync(scan, cancellationToken);

            scans.Add(new DeviceComplianceScanDto
            {
                Id = scan.Id,
                TenantId = scan.TenantId,
                DeviceId = scan.DeviceId,
                DeviceName = device.Name,
                DeviceIpAddress = device.IpAddress,
                ScannedAtUtc = scan.ScannedAtUtc,
                OverallStatus = scan.OverallStatus,
                PassedChecks = scan.PassedChecks,
                FailedChecks = scan.FailedChecks,
                WarningChecks = scan.WarningChecks,
                NotApplicableChecks = scan.NotApplicableChecks,
                TotalChecks = scan.TotalChecks,
                EvaluationNotes = scan.EvaluationNotes,
                Results = scan.Results.Select(r => new DeviceComplianceResultDto
                {
                    Id = r.Id,
                    ScanId = r.ScanId,
                    PolicyId = r.PolicyId,
                    PolicyName = applicablePolicies.FirstOrDefault(p => p.Id == r.PolicyId)?.Name ?? "Unknown Policy",
                    CheckType = r.CheckType,
                    Category = r.Category,
                    Severity = r.Severity,
                    Status = r.Status,
                    Summary = r.Summary,
                    Details = r.Details,
                    RemediationGuidance = r.RemediationGuidance,
                    EvaluatedAtUtc = r.EvaluatedAtUtc
                }).ToList()
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return scans;
    }
}