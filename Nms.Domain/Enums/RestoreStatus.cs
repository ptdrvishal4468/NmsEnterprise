namespace Nms.Domain.Enums;

public enum RestoreStatus
{
    Pending = 0,
    InProgress = 1,
    Success = 2,
    Failed = 3,
    RolledBack = 4,
    RollbackFailed = 5
}