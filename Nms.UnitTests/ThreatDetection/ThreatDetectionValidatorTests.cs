using Nms.Application.ThreatDetection.Commands.AnalyzeConfigurationDrift;
using Nms.Application.ThreatDetection.Commands.CreateThreatRule;
using Nms.Application.ThreatDetection.Commands.UpdateThreatIndicatorStatus;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.ThreatDetection;

public class ThreatDetectionValidatorTests
{
    [Fact]
    public void CreateThreatRuleCommandValidator_ShouldFail_WhenNameIsEmptyOrThresholdIsZero()
    {
        var validator = new CreateThreatRuleCommandValidator();
        var command = new CreateThreatRuleCommand(
            RuleName: "",
            ThreatType: ThreatType.FailedLogin,
            DefaultSeverity: ThreatSeverity.High,
            FailureThreshold: 0,
            TimeWindowMinutes: 0);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.RuleName));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.FailureThreshold));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.TimeWindowMinutes));
    }

    [Fact]
    public void CreateThreatRuleCommandValidator_ShouldPass_WhenInputIsValid()
    {
        var validator = new CreateThreatRuleCommandValidator();
        var command = new CreateThreatRuleCommand(
            RuleName: "Valid Rule",
            ThreatType: ThreatType.FailedLogin,
            DefaultSeverity: ThreatSeverity.High,
            FailureThreshold: 5,
            TimeWindowMinutes: 15);

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void UpdateThreatIndicatorStatusCommandValidator_ShouldFail_WhenIdIsEmpty()
    {
        var validator = new UpdateThreatIndicatorStatusCommandValidator();
        var command = new UpdateThreatIndicatorStatusCommand(Guid.Empty, ThreatStatus.Resolved);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Id));
    }

    [Fact]
    public void AnalyzeConfigurationDriftCommandValidator_ShouldFail_WhenDeviceIdIsEmptyGuid()
    {
        var validator = new AnalyzeConfigurationDriftCommandValidator();
        var command = new AnalyzeConfigurationDriftCommand(Guid.Empty);

        var result = validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.DeviceId));
    }

    [Fact]
    public void AnalyzeConfigurationDriftCommandValidator_ShouldPass_WhenDeviceIdIsNull()
    {
        var validator = new AnalyzeConfigurationDriftCommandValidator();
        var command = new AnalyzeConfigurationDriftCommand(null);

        var result = validator.Validate(command);

        Assert.True(result.IsValid);
    }
}