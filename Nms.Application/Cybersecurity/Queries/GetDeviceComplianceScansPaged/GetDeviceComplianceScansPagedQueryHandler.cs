using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Cybersecurity.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Cybersecurity.Queries.GetDeviceComplianceScansPaged;

public class GetDeviceComplianceScansPagedQueryHandler : IRequestHandler<GetDeviceComplianceScansPagedQuery, PagedResult<DeviceComplianceScanDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDeviceComplianceScansPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<DeviceComplianceScanDto>> Handle(GetDeviceComplianceScansPagedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.DeviceComplianceScans.GetPagedAsync(
            request.PageIndex,
            request.PageSize,
            request.DeviceId,
            request.OverallStatus,
            request.FromDateUtc,
            request.ToDateUtc,
            cancellationToken);

        var dtos = items.Select(scan => new DeviceComplianceScanDto
        {
            Id = scan.Id,
            TenantId = scan.TenantId,
            DeviceId = scan.DeviceId,
            DeviceName = scan.Device?.Name,
            DeviceIpAddress = scan.Device?.IpAddress,
            ScannedAtUtc = scan.ScannedAtUtc,
            OverallStatus = scan.OverallStatus,
            PassedChecks = scan.PassedChecks,
            FailedChecks = scan.FailedChecks,
            WarningChecks = scan.WarningChecks,
            NotApplicableChecks = scan.NotApplicableChecks,
            TotalChecks = scan.TotalChecks,
            EvaluationNotes = scan.EvaluationNotes
        }).ToList();

        return new PagedResult<DeviceComplianceScanDto>(dtos, totalCount, request.PageIndex, request.PageSize);
    }
}