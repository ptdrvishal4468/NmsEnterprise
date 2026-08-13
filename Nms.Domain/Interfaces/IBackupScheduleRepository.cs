using Nms.Domain.Entities;

namespace Nms.Domain.Interfaces;

public interface IBackupScheduleRepository : IGenericRepository<BackupSchedule, Guid>
{
    Task<(IReadOnlyList<BackupSchedule> Items, int TotalCount)> GetSchedulesPagedAsync(
        Guid tenantId,
        Guid? deviceId,
        bool? isEnabled,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BackupSchedule>> GetDueSchedulesAsync(
        CancellationToken cancellationToken = default);
}