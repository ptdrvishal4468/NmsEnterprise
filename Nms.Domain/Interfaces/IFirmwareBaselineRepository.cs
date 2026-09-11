using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IFirmwareBaselineRepository : IGenericRepository<FirmwareBaseline, Guid>
{
    Task<FirmwareBaseline?> GetByVendorAndModelAsync(Guid tenantId, string vendor, string model, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FirmwareBaseline>> GetActiveBaselinesAsync(Guid tenantId, CancellationToken cancellationToken = default);
}