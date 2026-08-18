namespace Nms.Domain.Enums;

public enum ComplianceCheckType
{
    SecureConfiguration = 1,
    PasswordComplexity = 2,
    TransportEncryption = 3,
    SnmpV3Security = 4,
    InsecureProtocolDisabled = 5,
    DefaultCredentialDisabled = 6
}