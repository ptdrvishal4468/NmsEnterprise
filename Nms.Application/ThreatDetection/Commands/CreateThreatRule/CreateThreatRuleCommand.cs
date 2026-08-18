using MediatR;
using Nms.Application.ThreatDetection.Dtos;
using Nms.Domain.Enums;

namespace Nms.Application.ThreatDetection.Commands.CreateThreatRule;

public record CreateThreatRuleCommand(
    string RuleName,
    ThreatType ThreatType,
    ThreatSeverity DefaultSeverity,
    int FailureThreshold,
    int TimeWindowMinutes,
    bool IsEnabled = true,
    string? Description = null) : IRequest<ThreatDetectionRuleDto>;