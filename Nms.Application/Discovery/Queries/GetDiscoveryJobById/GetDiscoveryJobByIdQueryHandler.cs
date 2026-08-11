using MediatR;
using Nms.Application.Discovery.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.Discovery.Queries.GetDiscoveryJobById;

public class GetDiscoveryJobByIdQueryHandler : IRequestHandler<GetDiscoveryJobByIdQuery, DiscoveryJobDto?>
{
    private readonly IDiscoveryJobRepository _jobRepository;

    public GetDiscoveryJobByIdQueryHandler(IDiscoveryJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public async Task<DiscoveryJobDto?> Handle(GetDiscoveryJobByIdQuery request, CancellationToken cancellationToken)
    {
        var job = await _jobRepository.GetWithCandidatesAsync(request.JobId, cancellationToken);
        if (job == null) return null;

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