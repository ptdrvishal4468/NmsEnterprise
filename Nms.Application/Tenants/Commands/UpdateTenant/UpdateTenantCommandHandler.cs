using MediatR;
using Nms.Application.Tenants.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, TenantDto?>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTenantCommandHandler(
        ITenantRepository tenantRepository,
        IUnitOfWork unitOfWork)
    {
        _tenantRepository = tenantRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TenantDto?> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.Id, cancellationToken);
        if (tenant == null)
        {
            return null;
        }

        // Domain method updates
        tenant.UpdateName(request.Name);

        if (request.IsActive && !tenant.IsActive)
        {
            tenant.Activate();
        }
        else if (!request.IsActive && tenant.IsActive)
        {
            tenant.Deactivate();
        }

        _tenantRepository.Update(tenant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TenantDto
        {
            Id = tenant.Id,
            Name = tenant.Name,
            IsActive = tenant.IsActive,
            CreatedAtUtc = tenant.CreatedAtUtc
        };
    }
}