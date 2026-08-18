using MediatR;
using Nms.Application.Cybersecurity.Dtos;

namespace Nms.Application.Cybersecurity.Queries.GetCybersecurityPostureSummary;

public record GetCybersecurityPostureSummaryQuery() : IRequest<CybersecurityPostureSummaryDto>;