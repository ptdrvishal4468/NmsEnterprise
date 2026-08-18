using MediatR;

namespace Nms.Application.Cybersecurity.Commands.DeleteCompliancePolicy;

public record DeleteCompliancePolicyCommand(Guid Id) : IRequest<bool>;