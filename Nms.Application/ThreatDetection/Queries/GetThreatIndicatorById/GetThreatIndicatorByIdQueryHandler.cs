using MediatR;
using Nms.Application.ThreatDetection.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.ThreatDetection.Queries.GetThreatIndicatorById;

public class GetThreatIndicatorByIdQueryHandler : IRequestHandler<GetThreatIndicatorByIdQuery, ThreatIndicatorDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetThreatIndicatorByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ThreatIndicatorDto?> Handle(GetThreatIndicatorByIdQuery request, CancellationToken cancellationToken)
    {
        var i = await _unitOfWork.ThreatIndicators.GetByIdAsync(request.Id, cancellationToken);
        if (i == null) return null;

        return new ThreatIndicatorDto
        {
            Id = i.Id,
            TenantId = i.TenantId,
            ThreatType = i.ThreatType,
            Severity = i.Severity,
            Status = i.Status,
            Title = i.Title,
            Description = i.Description,
            SourceIp = i.SourceIp,
            TargetDeviceId = i.TargetDeviceId,
            TargetDeviceName = i.TargetDevice?.Name,
            TargetUser = i.TargetUser,
            AttemptCount = i.AttemptCount,
            FirstDetectedAtUtc = i.FirstDetectedAtUtc,
            LastDetectedAtUtc = i.LastDetectedAtUtc,
            ResolutionNotes = i.ResolutionNotes,
            IndicatorMetadataJson = i.IndicatorMetadataJson
        };
    }
}