using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.ThreatDetection;

public class ThreatDetectionDomainTests
{
    [Fact]
    public void ThreatIndicator_DefaultValues_ShouldBeCorrect()
    {
        var tenantId = Guid.NewGuid();
        var indicator = new ThreatIndicator
        {
            TenantId = tenantId,
            ThreatType = ThreatType.FailedLogin,
            Severity = ThreatSeverity.High,
            Title = "Repeated Failed Login",
            Description = "5 attempts from 10.0.0.1",
            SourceIp = "10.0.0.1"
        };

        Assert.Equal(tenantId, indicator.TenantId);
        Assert.Equal(ThreatStatus.Active, indicator.Status);
        Assert.Equal(1, indicator.AttemptCount);
        Assert.True(indicator.FirstDetectedAtUtc <= DateTime.UtcNow);
        Assert.True(indicator.LastDetectedAtUtc <= DateTime.UtcNow);
    }

    [Fact]
    public void ThreatDetectionRule_DefaultValues_ShouldBeCorrect()
    {
        var tenantId = Guid.NewGuid();
        var rule = new ThreatDetectionRule
        {
            TenantId = tenantId,
            RuleName = "Default Failed Login Rule",
            ThreatType = ThreatType.FailedLogin,
            DefaultSeverity = ThreatSeverity.High
        };

        Assert.Equal(5, rule.FailureThreshold);
        Assert.Equal(15, rule.TimeWindowMinutes);
        Assert.True(rule.IsEnabled);
    }

    [Fact]
    public void ConfigurationDriftRecord_Invariants_ShouldHold()
    {
        var tenantId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var baselineId = Guid.NewGuid();
        var currentId = Guid.NewGuid();

        var drift = new ConfigurationDriftRecord
        {
            TenantId = tenantId,
            DeviceId = deviceId,
            BaselineBackupId = baselineId,
            CurrentBackupId = currentId,
            HasDrift = true,
            AddedLinesCount = 5,
            RemovedLinesCount = 2,
            ModifiedLinesCount = 2,
            Severity = ThreatSeverity.Medium
        };

        Assert.True(drift.HasDrift);
        Assert.Equal(5, drift.AddedLinesCount);
        Assert.Equal(2, drift.RemovedLinesCount);
        Assert.False(drift.IsAcknowledged);
    }
}