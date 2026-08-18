using MediatR;
using Nms.Application.ThreatDetection.Dtos;

namespace Nms.Application.ThreatDetection.Commands.AnalyzeConfigurationDrift;

public record AnalyzeConfigurationDriftCommand(Guid? DeviceId = null) : IRequest<IReadOnlyList<ConfigurationDriftRecordDto>>;