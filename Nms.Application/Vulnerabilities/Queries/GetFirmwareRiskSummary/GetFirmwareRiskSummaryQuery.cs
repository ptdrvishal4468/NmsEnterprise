using MediatR;
using Nms.Application.Vulnerabilities.Dtos;

namespace Nms.Application.Vulnerabilities.Queries.GetFirmwareRiskSummary;

public record GetFirmwareRiskSummaryQuery : IRequest<FirmwareRiskSummaryDto>;