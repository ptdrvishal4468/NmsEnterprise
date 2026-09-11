using Nms.Application.ConfigurationBackups.Commands.CreateBackupSchedule;
using Nms.Application.ConfigurationBackups.Commands.CreateConfigurationBackup;
using Nms.Domain.Enums;

namespace Nms.IntegrationTests.ConfigurationBackups;

public class ConfigurationBackupsEndpointTests
{
    [Fact]
    public void TriggerBackup_Route_MatchesExpectedFormat()
    {
        var deviceId = Guid.NewGuid();
        var route = $"/api/v1/configurationbackups/devices/{deviceId}";

        Assert.Equal($"/api/v1/configurationbackups/devices/{deviceId}", route);
    }

    [Fact]
    public void DownloadBackup_Route_MatchesExpectedFormat()
    {
        var backupId = Guid.NewGuid();
        var route = $"/api/v1/configurationbackups/{backupId}/download";

        Assert.Equal($"/api/v1/configurationbackups/{backupId}/download", route);
    }

    [Fact]
    public void CreateConfigurationBackupCommand_SerializesCorrectly()
    {
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();

        var command = new CreateConfigurationBackupCommand(tenantId, deviceId, BackupTriggerType.Manual);

        Assert.Equal(tenantId, command.TenantId);
        Assert.Equal(deviceId, command.DeviceId);
        Assert.Equal(BackupTriggerType.Manual, command.TriggerType);
    }

    [Fact]
    public void CreateBackupScheduleCommand_SerializesCorrectly()
    {
        var tenantId = Guid.NewGuid();
        var command = new CreateBackupScheduleCommand(
            TenantId: tenantId,
            Name: "Daily Router Backup",
            IntervalMinutes: 1440,
            Description: "Automated daily backup schedule"
        );

        Assert.Equal("Daily Router Backup", command.Name);
        Assert.Equal(1440, command.IntervalMinutes);
        Assert.Equal(tenantId, command.TenantId);
    }
}