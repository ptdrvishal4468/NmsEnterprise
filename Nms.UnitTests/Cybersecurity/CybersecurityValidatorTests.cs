using FluentAssertions;
using Nms.Application.Cybersecurity.Commands.CreateCompliancePolicy;
using Nms.Application.Cybersecurity.Commands.EvaluateDeviceCompliance;
using Nms.Application.Cybersecurity.Commands.UpdateCompliancePolicy;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Cybersecurity;

public class CybersecurityValidatorTests
{
    [Fact]
    public void CreateCompliancePolicyCommandValidator_WhenNameEmpty_FailsValidation()
    {
        var validator = new CreateCompliancePolicyCommandValidator();
        var command = new CreateCompliancePolicyCommand(
            string.Empty,
            "Description",
            ComplianceCategory.Configuration,
            ComplianceCheckType.SecureConfiguration,
            ComplianceSeverity.Medium);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateCompliancePolicyCommand.Name));
    }

    [Fact]
    public void UpdateCompliancePolicyCommandValidator_WhenIdEmpty_FailsValidation()
    {
        var validator = new UpdateCompliancePolicyCommandValidator();
        var command = new UpdateCompliancePolicyCommand(
            Guid.Empty,
            "Valid Name",
            "Description",
            ComplianceCategory.Configuration,
            ComplianceCheckType.SecureConfiguration,
            ComplianceSeverity.Medium,
            true);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateCompliancePolicyCommand.Id));
    }

    [Fact]
    public void EvaluateDeviceComplianceCommandValidator_WhenDeviceIdEmpty_FailsValidation()
    {
        var validator = new EvaluateDeviceComplianceCommandValidator();
        var command = new EvaluateDeviceComplianceCommand(Guid.Empty);

        var result = validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(EvaluateDeviceComplianceCommand.DeviceId));
    }
}