using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Discovery.Dtos;
using Nms.Application.Discovery.Services;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Discovery.Commands.StartDiscoveryScan;

public class StartDiscoveryScanCommandHandler : IRequestHandler<StartDiscoveryScanCommand, DiscoveryJobDto>
{
    private readonly ITenantContext _tenantContext;
    private readonly IDiscoveryJobRepository _jobRepository;
    private readonly DeviceDiscoveryEngine _discoveryEngine;
    private readonly IUnitOfWork _unitOfWork;

    public StartDiscoveryScanCommandHandler(
        ITenantContext tenantContext,
        IDiscoveryJobRepository jobRepository,
        DeviceDiscoveryEngine discoveryEngine,
        IUnitOfWork unitOfWork)
    {
        _tenantContext = tenantContext;
        _jobRepository = jobRepository;
        _discoveryEngine = discoveryEngine;
        _unitOfWork = unitOfWork;
    }

    public async Task<DiscoveryJobDto> Handle(StartDiscoveryScanCommand request, CancellationToken cancellationToken)
    {
        var job = new DiscoveryJob(
            id: Guid.NewGuid(),
            tenantId: _tenantContext.TenantId,
            name: request.Name,
            ipRange: request.IpRange,
            snmpCommunity: request.SnmpCommunity,
            snmpPort: request.SnmpPort);

        await _discoveryEngine.ExecuteScanAsync(job, maxConcurrency: 20, cancellationToken);

        await _jobRepository.AddAsync(job, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(job);
    }

    private static DiscoveryJobDto MapToDto(DiscoveryJob job)
    {
        var candidates = job.Candidates.Select(c => new DiscoveredDeviceCandidateDto(
            c.Id,
            c.DiscoveryJobId,
            c.IpAddress,
            c.IsIcmpReachable,
            c.ResponseTimeMs,
            c.IsSnmpReachable,
            c.SysDescr,
            c.SysObjectId,
            c.SysName,
            c.FingerprintedType,
            c.IdentifiedVendor,
            c.IsDuplicate,
            c.ExistingDeviceId,
            c.IsImported,
            c.ImportedDeviceId)).ToList();

        return new DiscoveryJobDto(
            job.Id,
            job.TenantId,
            job.Name,
            job.IpRange,
            job.Status,
            job.TotalTargets,
            job.ProcessedTargets,
            job.DiscoveredCount,
            job.StartedAtUtc,
            job.CompletedAtUtc,
            job.FailureReason,
            candidates);
    }
}