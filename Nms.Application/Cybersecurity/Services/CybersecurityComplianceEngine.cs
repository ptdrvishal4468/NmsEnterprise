using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;

namespace Nms.Application.Cybersecurity.Services;

public class CybersecurityComplianceEngine : ICybersecurityComplianceEngine
{
    private readonly ISecureConfigurationValidator _configurationValidator;
    private readonly IPasswordPolicyVerifier _passwordPolicyVerifier;
    private readonly IEncryptionVerifier _encryptionVerifier;

    public CybersecurityComplianceEngine(
        ISecureConfigurationValidator configurationValidator,
        IPasswordPolicyVerifier passwordPolicyVerifier,
        IEncryptionVerifier encryptionVerifier)
    {
        _configurationValidator = configurationValidator;
        _passwordPolicyVerifier = passwordPolicyVerifier;
        _encryptionVerifier = encryptionVerifier;
    }

    public async Task<DeviceComplianceScan> EvaluateDeviceAsync(
        Device device,
        IReadOnlyList<CompliancePolicy> policies,
        string? evaluationNotes = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(policies);

        var scanId = Guid.NewGuid();
        var scan = new DeviceComplianceScan(
            id: scanId,
            tenantId: device.TenantId,
            deviceId: device.Id,
            scannedAtUtc: DateTime.UtcNow,
            evaluationNotes: evaluationNotes);

        foreach (var policy in policies.Where(p => p.IsActive))
        {
            DeviceComplianceResult result = policy.Category switch
            {
                ComplianceCategory.Configuration or ComplianceCategory.ProtocolSecurity
                    => await _configurationValidator.ValidateConfigurationAsync(device, policy, scanId, cancellationToken),

                ComplianceCategory.PasswordPolicy or ComplianceCategory.AccessControl
                    => await _passwordPolicyVerifier.VerifyPasswordPolicyAsync(device, policy, scanId, cancellationToken),

                ComplianceCategory.Encryption
                    => await _encryptionVerifier.VerifyEncryptionAsync(device, policy, scanId, cancellationToken),

                _ => await _configurationValidator.ValidateConfigurationAsync(device, policy, scanId, cancellationToken)
            };

            scan.AddResult(result);
        }

        scan.RecalculateTotals();
        return scan;
    }
}