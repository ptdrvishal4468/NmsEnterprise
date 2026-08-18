using MediatR;
using Nms.Application.Cybersecurity.Dtos;

namespace Nms.Application.Cybersecurity.Queries.GetDeviceLatestComplianceScan;

public record GetDeviceLatestComplianceScanQuery(Guid DeviceId) : IRequest<DeviceComplianceScanDto?>;