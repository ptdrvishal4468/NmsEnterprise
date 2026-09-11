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
    public ITopologyLinkRepository TopologyLinks { get; }
    public IScheduledReportRepository ScheduledReports { get; }
    public IScheduledReportExecutionLogRepository ScheduledReportExecutionLogs { get; }
    public IAuditLogRepository AuditLogs { get; }
    public IAssetRepository Assets { get; }
    public ISiteRepository Sites { get; }
    public IBuildingRepository Buildings { get; }
    public IFloorRepository Floors { get; }
    public IRoomRepository Rooms { get; }
    public IRackRepository Racks { get; }
    public ICustomerRepository Customers { get; }
    public ICustomerContactRepository CustomerContacts { get; }
    public ITicketRepository Tickets { get; }
    public ITicketSyncLogRepository TicketSyncLogs { get; }
    public ICompliancePolicyRepository CompliancePolicies { get; }
    public IDeviceComplianceScanRepository DeviceComplianceScans { get; }
    public IVulnerabilityRepository Vulnerabilities { get; }
    public ISecurityAdvisoryRepository SecurityAdvisories { get; }
    public IDeviceVulnerabilityMatchRepository DeviceVulnerabilityMatches { get; }
    public IFirmwareUpgradeRecommendationRepository FirmwareUpgradeRecommendations { get; }
    public IThreatIndicatorRepository ThreatIndicators { get; }
    public IThreatDetectionRuleRepository ThreatDetectionRules { get; }
    public IConfigurationDriftRepository ConfigurationDrifts { get; }
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
        TopologyLinks = new TopologyLinkRepository(_context);
        ScheduledReports = new ScheduledReportRepository(_context);
        ScheduledReportExecutionLogs = new ScheduledReportExecutionLogRepository(_context);
        AuditLogs = new AuditLogRepository(_context);
        Assets = new AssetRepository(_context);
        Sites = new SiteRepository(_context);
        Buildings = new BuildingRepository(_context);
        Floors = new FloorRepository(_context);
        Rooms = new RoomRepository(_context);
        Racks = new RackRepository(_context);
        Customers = new CustomerRepository(_context);
        CustomerContacts = new CustomerContactRepository(_context);
        Tickets = new TicketRepository(_context);
        TicketSyncLogs = new TicketSyncLogRepository(_context);
        CompliancePolicies = new CompliancePolicyRepository(_context);
        DeviceComplianceScans = new DeviceComplianceScanRepository(_context);
        Vulnerabilities = new VulnerabilityRepository(_context);
        SecurityAdvisories = new SecurityAdvisoryRepository(_context);
        DeviceVulnerabilityMatches = new DeviceVulnerabilityMatchRepository(_context);
        FirmwareUpgradeRecommendations = new FirmwareUpgradeRecommendationRepository(_context);
        ThreatIndicators = new ThreatIndicatorRepository(_context);
        ThreatDetectionRules = new ThreatDetectionRuleRepository(_context);
        ConfigurationDrifts = new ConfigurationDriftRepository(_context);

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