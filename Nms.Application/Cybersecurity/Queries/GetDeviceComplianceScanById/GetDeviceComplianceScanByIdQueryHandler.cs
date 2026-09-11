using MediatR;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Cybersecurity.Queries.GetDeviceComplianceScanById;

public class GetDeviceComplianceScanByIdQueryHandler : IRequestHandler<GetDeviceComplianceScanByIdQuery, DeviceComplianceScanDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDeviceComplianceScanByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeviceComplianceScanDto?> Handle(GetDeviceComplianceScanByIdQuery request, CancellationToken cancellationToken)
    {
        var scan = await _unitOfWork.DeviceComplianceScans.GetScanWithResultsAsync(request.ScanId, cancellationToken);
        if (scan == null)
            return null;

        var device = scan.Device ?? await _unitOfWork.Devices.GetByIdAsync(scan.DeviceId, cancellationToken);

        return new DeviceComplianceScanDto
        {
            Id = scan.Id,
            TenantId = scan.TenantId,
            DeviceId = scan.DeviceId,
            DeviceName = device?.Name,
            DeviceIpAddress = device?.IpAddress,
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
                PolicyName = r.Policy?.Name ?? "Security Policy",
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