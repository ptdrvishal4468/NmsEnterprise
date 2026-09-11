namespace Nms.Application.Cybersecurity.Dtos;

public class EvaluateDeviceComplianceDto
{
    public Guid DeviceId { get; set; }
    public string? EvaluationNotes { get; set; }
    public IReadOnlyList<Guid>? SpecificPolicyIds { get; set; }
}