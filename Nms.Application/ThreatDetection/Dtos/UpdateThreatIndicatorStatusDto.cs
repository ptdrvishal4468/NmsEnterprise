using Nms.Domain.Enums;

namespace Nms.Application.ThreatDetection.Dtos;

public class UpdateThreatIndicatorStatusDto
{
    public ThreatStatus Status { get; set; }
    public string? ResolutionNotes { get; set; }
}