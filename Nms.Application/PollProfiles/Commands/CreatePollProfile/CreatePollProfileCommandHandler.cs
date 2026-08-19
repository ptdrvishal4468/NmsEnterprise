using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;

namespace Nms.Application.PollProfiles.Commands.CreatePollProfile;

public class CreatePollProfileCommandHandler : IRequestHandler<CreatePollProfileCommand, Guid>
{
    private readonly IPollProfileRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly ICacheService? _cacheService;

    public CreatePollProfileCommandHandler(
        IPollProfileRepository repository,
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        ICacheService? cacheService = null)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _cacheService = cacheService;
    }

    public async Task<Guid> Handle(CreatePollProfileCommand request, CancellationToken cancellationToken)
    {
        var tenantId = _tenantContext.TenantId;

        var profile = new PollProfile(
            id: Guid.NewGuid(),
            tenantId: tenantId,
            name: request.Dto.Name,
            intervalSeconds: request.Dto.IntervalSeconds,
            timeoutSeconds: request.Dto.TimeoutSeconds,
            retryCount: request.Dto.RetryCount,
            isDefault: request.Dto.IsDefault,
            description: request.Dto.Description
        );

        profile.SetCreatedAudit(tenantId.ToString());

        await _repository.AddAsync(profile, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (_cacheService != null)
        {
            await _cacheService.RemoveByPrefixAsync("rules:poll-profiles", cancellationToken);
        }

        return profile.Id;
    }
}