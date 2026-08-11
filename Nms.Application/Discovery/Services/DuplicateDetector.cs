using Nms.Domain.Interfaces;

namespace Nms.Application.Discovery.Services;

public class DuplicateDetector
{
    private readonly IDeviceRepository _deviceRepository;

    public DuplicateDetector(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<(bool IsDuplicate, Guid? ExistingDeviceId)> DetectAsync(
        string ipAddress,
        CancellationToken cancellationToken = default)
    {
        var existingDevice = await _deviceRepository.GetByIpAddressAsync(ipAddress, cancellationToken);
        if (existingDevice != null)
        {
            return (true, existingDevice.Id);
        }

        return (false, null);
    }
}