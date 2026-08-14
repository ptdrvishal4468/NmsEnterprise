namespace Nms.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern interface managing database transaction boundaries.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IDeviceRepository Devices { get; }
    IReachabilityHistoryRepository ReachabilityHistories { get; }
    IEventRepository Events { get; }
    ISyslogRepository Syslogs { get; }
    ISnmpTrapRepository SnmpTraps { get; }
    IConfigurationBackupRepository ConfigurationBackups { get; }
    IBackupScheduleRepository BackupSchedules { get; }
    IConfigurationRestoreLogRepository ConfigurationRestoreLogs { get; }
    IFirmwareBaselineRepository FirmwareBaselines { get; }
    IFirmwareUpgradePlanRepository FirmwareUpgradePlans { get; }
    ITopologyLinkRepository TopologyLinks { get; }

    /// <summary>
    /// Asynchronously commits all pending tracking changes to the database context.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a formal database transaction for multi-step operations.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the active database transaction.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the active database transaction upon failure.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}