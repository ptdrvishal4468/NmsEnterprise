using MediatR;
using Nms.Application.Firmware.Dtos;

namespace Nms.Application.Firmware.Queries.GetFirmwareComplianceSummary;

public record GetFirmwareComplianceSummaryQuery : IRequest<FirmwareComplianceSummaryDto>;