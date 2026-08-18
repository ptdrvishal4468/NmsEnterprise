using MediatR;
using Nms.Application.Cybersecurity.Dtos;

namespace Nms.Application.Cybersecurity.Queries.GetDeviceComplianceScanById;

public record GetDeviceComplianceScanByIdQuery(Guid ScanId) : IRequest<DeviceComplianceScanDto?>;