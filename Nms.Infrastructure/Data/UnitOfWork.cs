using Microsoft.EntityFrameworkCore.Storage;
using Nms.Domain.Interfaces;
using Nms.Infrastructure.Data.Repositories;

namespace Nms.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly NmsDbContext _context;
    private IDbContextTransaction? _transaction;

    public IDeviceRepository Devices { get; }
    public IReachabilityHistoryRepository ReachabilityHistories { get; }
    public IEventRepository Events { get; }
    public ISyslogRepository Syslogs { get; }
    public ISnmpTrapRepository SnmpTraps { get; }
    public IConfigurationBackupRepository ConfigurationBackups { get; }
    public IBackupScheduleRepository BackupSchedules { get; }
    public IConfigurationRestoreLogRepository ConfigurationRestoreLogs { get; }
    public IFirmwareBaselineRepository FirmwareBaselines { get; }
    public IFirmwareUpgradePlanRepository FirmwareUpgradePlans { get; }

    public UnitOfWork(NmsDbContext context)
    {
        _context = context;
        Devices = new DeviceRepository(_context);
        ReachabilityHistories = new ReachabilityHistoryRepository(_context);
        Events = new EventRepository(_context);
        Syslogs = new SyslogRepository(_context);
        SnmpTraps = new SnmpTrapRepository(_context);
        ConfigurationBackups = new ConfigurationBackupRepository(_context);
        BackupSchedules = new BackupScheduleRepository(_context);
        ConfigurationRestoreLogs = new ConfigurationRestoreLogRepository(_context);
        FirmwareBaselines = new FirmwareBaselineRepository(_context);
        FirmwareUpgradePlans = new FirmwareUpgradePlanRepository(_context);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}