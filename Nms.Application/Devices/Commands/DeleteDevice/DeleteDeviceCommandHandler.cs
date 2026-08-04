using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Interfaces;

namespace Nms.Application.Devices.Commands.DeleteDevice;

public class DeleteDeviceCommandHandler : IRequestHandler<DeleteDeviceCommand, bool>
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ITenantContext _tenantContext;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDeviceCommandHandler(
        IDeviceRepository deviceRepository,
        ITenantContext tenantContext,
        IUnitOfWork unitOfWork)
    {
        _deviceRepository = deviceRepository;
        _tenantContext = tenantContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteDeviceCommand request, CancellationToken cancellationToken)
    {
        if (!_tenantContext.IsResolved || _tenantContext.TenantId == Guid.Empty)
        {
            throw new InvalidOperationException("A valid tenant context is required to delete a device.");
        }

        var device = await _deviceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (device == null)
        {
            return false;
        }

        _deviceRepository.Remove(device);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}