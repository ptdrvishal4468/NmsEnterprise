using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Cybersecurity.Commands.EvaluateDeviceCompliance;

public class EvaluateDeviceComplianceCommandHandler : IRequestHandler<EvaluateDeviceComplianceCommand, DeviceComplianceScanDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICybersecurityComplianceEngine _complianceEngine;

    public EvaluateDeviceComplianceCommandHandler(
        IUnitOfWork unitOfWork,
        ICybersecurityComplianceEngine complianceEngine)
    {
        _unitOfWork = unitOfWork;
        _complianceEngine = complianceEngine;
    }

    public async Task<DeviceComplianceScanDto?> Handle(EvaluateDeviceComplianceCommand request, CancellationToken cancellationToken)
    {
        var device = await _unitOfWork.Devices.GetByIdAsync(request.DeviceId, cancellationToken);
        if (device == null)
            return null;

        IReadOnlyList<CompliancePolicy> policiesToRun;

        if (request.SpecificPolicyIds != null && request.SpecificPolicyIds.Count > 0)
        {
            var allPolicies = await _unitOfWork.CompliancePolicies.GetActivePoliciesAsync(cancellationToken);
            policiesToRun = allPolicies.Where(p => request.SpecificPolicyIds.Contains(p.Id)).ToList();
        }
        else
        {
            policiesToRun = await _unitOfWork.CompliancePolicies.GetActivePoliciesForDeviceTypeAsync(
                device.DeviceType,
                device.Vendor,
                cancellationToken);
        }

        var scan = await _complianceEngine.EvaluateDeviceAsync(
            device,
            policiesToRun,
            request.EvaluationNotes,
            cancellationToken);

        await _unitOfWork.DeviceComplianceScans.AddAsync(scan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeviceComplianceScanDto
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
                PolicyName = policiesToRun.FirstOrDefault(p => p.Id == r.PolicyId)?.Name ?? "Unknown Policy",
                CheckType = r.CheckType,
                Category = r.Category,
                Severity = r.Severity,
                Status = r.Status,
                Summary = r.Summary,
                Details = r.Details,
                RemediationGuidance = r.RemediationGuidance,
                EvaluatedAtUtc = r.EvaluatedAtUtc
            }).ToList()
        };
    }
}