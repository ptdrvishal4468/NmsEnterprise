namespace Nms.Domain.Enums;

/// <summary>
/// Defines the outcome state of an audited operation.
/// </summary>
public enum AuditStatus
{
    Success = 1,
    Failure = 2,
    Warning = 3
}