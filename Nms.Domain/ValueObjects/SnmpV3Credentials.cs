using Nms.Domain.Enums;

namespace Nms.Domain.ValueObjects;

public sealed record SnmpV3Credentials
{
    public string Username { get; }
    public SnmpSecurityLevel SecurityLevel { get; }
    public SnmpAuthProtocol AuthProtocol { get; }
    public string? AuthPassword { get; }
    public SnmpPrivProtocol PrivProtocol { get; }
    public string? PrivPassword { get; }
    public string? EngineId { get; }

    public SnmpV3Credentials(
        string username,
        SnmpSecurityLevel securityLevel,
        SnmpAuthProtocol authProtocol = SnmpAuthProtocol.None,
        string? authPassword = null,
        SnmpPrivProtocol privProtocol = SnmpPrivProtocol.None,
        string? privPassword = null,
        string? engineId = null)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        }

        if (securityLevel >= SnmpSecurityLevel.AuthNoPriv && string.IsNullOrWhiteSpace(authPassword))
        {
            throw new ArgumentException("AuthPassword is required when SecurityLevel is AuthNoPriv or AuthPriv.", nameof(authPassword));
        }

        if (securityLevel >= SnmpSecurityLevel.AuthNoPriv && authProtocol == SnmpAuthProtocol.None)
        {
            throw new ArgumentException("A valid AuthProtocol must be selected for AuthNoPriv or AuthPriv.", nameof(authProtocol));
        }

        if (securityLevel == SnmpSecurityLevel.AuthPriv && string.IsNullOrWhiteSpace(privPassword))
        {
            throw new ArgumentException("PrivPassword is required when SecurityLevel is AuthPriv.", nameof(privPassword));
        }

        if (securityLevel == SnmpSecurityLevel.AuthPriv && privProtocol == SnmpPrivProtocol.None)
        {
            throw new ArgumentException("A valid PrivProtocol must be selected for AuthPriv.", nameof(privProtocol));
        }

        Username = username;
        SecurityLevel = securityLevel;
        AuthProtocol = authProtocol;
        AuthPassword = authPassword;
        PrivProtocol = privProtocol;
        PrivPassword = privPassword;
        EngineId = engineId;
    }
}