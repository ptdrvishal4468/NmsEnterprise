using MediatR;
using Nms.Application.Dashboard.Dtos;

namespace Nms.Application.Dashboard.Queries.GetDeviceStatistics;

public sealed record GetDeviceStatisticsQuery : IRequest<DeviceStatisticsDto>;