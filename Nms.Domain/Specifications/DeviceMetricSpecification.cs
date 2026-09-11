using Nms.Domain.Entities;

namespace Nms.Domain.Specifications;

public class DeviceMetricSpecification : BaseSpecification<DeviceMetricRaw>
{
    public DeviceMetricSpecification(Guid deviceId, DateTime fromUtc, DateTime toUtc)
        : base(m => m.DeviceId == deviceId
                 && m.TimestampUtc >= fromUtc
                 && m.TimestampUtc <= toUtc)
    {
        ApplyPaging(0, 1000);
    }
}