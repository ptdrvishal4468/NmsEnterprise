using MediatR;
using Nms.Application.ThreatDetection.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.ThreatDetection.Commands.UpdateThreatIndicatorStatus;

public class UpdateThreatIndicatorStatusCommandHandler : IRequestHandler<UpdateThreatIndicatorStatusCommand, ThreatIndicatorDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateThreatIndicatorStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ThreatIndicatorDto> Handle(UpdateThreatIndicatorStatusCommand request, CancellationToken cancellationToken)
    {
        var indicator = await _unitOfWork.ThreatIndicators.GetByIdAsync(request.Id, cancellationToken);
        if (indicator == null)
            throw new KeyNotFoundException($"Threat indicator with ID '{request.Id}' was not found.");

        indicator.Status = request.Status;
        if (!string.IsNullOrWhiteSpace(request.ResolutionNotes))
            indicator.ResolutionNotes = request.ResolutionNotes;

        _unitOfWork.ThreatIndicators.Update(indicator);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ThreatIndicatorDto
        {
            Id = indicator.Id,
            TenantId = indicator.TenantId,
            ThreatType = indicator.ThreatType,
            Severity = indicator.Severity,
            Status = indicator.Status,
            Title = indicator.Title,
            Description = indicator.Description,
            SourceIp = indicator.SourceIp,
            TargetDeviceId = indicator.TargetDeviceId,
            TargetDeviceName = indicator.TargetDevice?.Name,
            TargetUser = indicator.TargetUser,
            AttemptCount = indicator.AttemptCount,
            FirstDetectedAtUtc = indicator.FirstDetectedAtUtc,
            LastDetectedAtUtc = indicator.LastDetectedAtUtc,
            ResolutionNotes = indicator.ResolutionNotes,
            IndicatorMetadataJson = indicator.IndicatorMetadataJson
        };
    }
}