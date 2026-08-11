using MediatR;
using Nms.Application.Discovery.Dtos;

namespace Nms.Application.Discovery.Commands.StartDiscoveryScan;

public record StartDiscoveryScanCommand(
    string Name,
    string IpRange,
    string? SnmpCommunity = "public",
    int SnmpPort = 161) : IRequest<DiscoveryJobDto>;