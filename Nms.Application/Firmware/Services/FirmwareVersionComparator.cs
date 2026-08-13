using System.Text.RegularExpressions;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;

namespace Nms.Application.Firmware.Services;

public class FirmwareVersionComparator : IFirmwareVersionComparator
{
    private static readonly Regex NumericDigitsRegex = new(@"\d+", RegexOptions.Compiled);

    public int Compare(string? version1, string? version2)
    {
        if (string.IsNullOrWhiteSpace(version1) && string.IsNullOrWhiteSpace(version2)) return 0;
        if (string.IsNullOrWhiteSpace(version1)) return -1;
        if (string.IsNullOrWhiteSpace(version2)) return 1;

        var v1Trimmed = version1.Trim();
        var v2Trimmed = version2.Trim();

        if (string.Equals(v1Trimmed, v2Trimmed, StringComparison.OrdinalIgnoreCase))
            return 0;

        if (Version.TryParse(CleanVersionString(v1Trimmed), out var parsedV1) &&
            Version.TryParse(CleanVersionString(v2Trimmed), out var parsedV2))
        {
            var cmpParsed = parsedV1.CompareTo(parsedV2);
            if (cmpParsed != 0)
                return cmpParsed;
        }

        var tokens1 = ExtractNumericSegments(v1Trimmed);
        var tokens2 = ExtractNumericSegments(v2Trimmed);

        var minSegments = Math.Min(tokens1.Count, tokens2.Count);
        for (int i = 0; i < minSegments; i++)
        {
            if (tokens1[i] != tokens2[i])
                return tokens1[i].CompareTo(tokens2[i]);
        }

        if (tokens1.Count != tokens2.Count)
        {
            return tokens1.Count.CompareTo(tokens2.Count);
        }

        return string.Compare(v1Trimmed, v2Trimmed, StringComparison.OrdinalIgnoreCase);
    }

    public FirmwareComplianceStatus EvaluateCompliance(string? currentVersion, string? targetVersion)
    {
        if (string.IsNullOrWhiteSpace(currentVersion) || string.IsNullOrWhiteSpace(targetVersion))
            return FirmwareComplianceStatus.Unknown;

        return Compare(currentVersion, targetVersion) == 0
            ? FirmwareComplianceStatus.Compliant
            : FirmwareComplianceStatus.NonCompliant;
    }

    private static string CleanVersionString(string input)
    {
        var match = Regex.Match(input, @"\d+(\.\d+)+");
        return match.Success ? match.Value : input;
    }

    private static List<int> ExtractNumericSegments(string input)
    {
        var matches = NumericDigitsRegex.Matches(input);
        var segments = new List<int>();

        foreach (Match match in matches)
        {
            if (int.TryParse(match.Value, out var num))
            {
                segments.Add(num);
            }
        }

        return segments;
    }
}