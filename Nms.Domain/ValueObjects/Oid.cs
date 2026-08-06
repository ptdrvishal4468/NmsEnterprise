using System.Text.RegularExpressions;

namespace Nms.Domain.ValueObjects;

public record Oid
{
    private static readonly Regex OidPattern = new(@"^(\.\d+)+$", RegexOptions.Compiled);

    public string Value { get; }

    public Oid(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("OID string cannot be null or empty.", nameof(value));
        }

        var normalized = value.Trim();
        if (!normalized.StartsWith('.'))
        {
            normalized = "." + normalized;
        }

        if (!OidPattern.IsMatch(normalized))
        {
            throw new ArgumentException($"Invalid OID string format: '{value}'. Must be dot-separated non-negative integers.", nameof(value));
        }

        Value = normalized;
    }

    public static Oid From(string value) => new(value);

    public bool IsPrefixOf(Oid target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return target.Value.StartsWith(Value + ".", StringComparison.Ordinal) || target.Value.Equals(Value, StringComparison.Ordinal);
    }

    public override string ToString() => Value;
}