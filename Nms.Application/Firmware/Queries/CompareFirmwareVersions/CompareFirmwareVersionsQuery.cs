using MediatR;
using Nms.Application.Firmware.Dtos;

namespace Nms.Application.Firmware.Queries.CompareFirmwareVersions;

public record CompareFirmwareVersionsQuery(
    string Version1,
    string Version2) : IRequest<FirmwareVersionComparisonDto>;