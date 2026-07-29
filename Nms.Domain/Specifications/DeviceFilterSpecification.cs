using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Domain.Specifications;

/// <summary>
/// Encapsulates complex filtering, pagination, and sorting logic for Device queries.
/// </summary>
public class DeviceFilterSpecification : BaseSpecification<Device>
{
    public DeviceFilterSpecification(
        DeviceType? type,
        DeviceStatus? status,
        string? searchTerm,
        int pageIndex,
        int pageSize)
        : base(d =>
            (!type.HasValue || d.DeviceType == type.Value) &&
            (!status.HasValue || d.Status == status.Value) &&
            (string.IsNullOrWhiteSpace(searchTerm) ||
             d.Name.Contains(searchTerm) ||
             d.IpAddress.Contains(searchTerm)))
    {
        ApplyOrderBy(d => d.Name);
        ApplyPaging((pageIndex - 1) * pageSize, pageSize);
    }
}