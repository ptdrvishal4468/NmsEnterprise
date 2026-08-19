using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Interfaces;

namespace Nms.Application.PollProfiles.Commands.UpdatePollProfile;

public class UpdatePollProfileCommandHandler : IRequestHandler<UpdatePollProfileCommand, bool>
{
    private readonly IPollProfileRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly ICacheService? _cacheService;

    public UpdatePollProfileCommandHandler(
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

    public async Task<bool> Handle(UpdatePollProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (profile == null)
            return false;

        profile.UpdateProfile(
            request.Dto.Name,
            request.Dto.IntervalSeconds,
            request.Dto.TimeoutSeconds,
            request.Dto.RetryCount,
            request.Dto.Description
        );

        profile.SetModifiedAudit(_tenantContext.TenantId.ToString());

        _repository.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (_cacheService != null)
        {
            await _cacheService.RemoveByPrefixAsync("rules:poll-profiles", cancellationToken);
        }

        return true;
    }
}