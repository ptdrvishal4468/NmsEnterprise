using Nms.Domain.Enums;

namespace Nms.Application.Common.Interfaces;

public interface IFirmwareVersionComparator
{
    /// <summary>
    /// Compares two firmware versions.
    /// Returns:
    ///  < 0 if version1 is older than version2
    ///    0 if version1 is equal to version2
    ///  > 0 if version1 is newer than version2
    /// </summary>
    int Compare(string? version1, string? version2);

    /// <summary>
    /// Evaluates if currentVersion meets targetVersion policy.
    /// </summary>
    FirmwareComplianceStatus EvaluateCompliance(string? currentVersion, string? targetVersion);
}