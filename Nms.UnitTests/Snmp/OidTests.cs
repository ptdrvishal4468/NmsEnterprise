using Nms.Domain.ValueObjects;
using Xunit;

namespace Nms.UnitTests.Snmp;

public class OidTests
{
    [Theory]
    [InlineData("1.3.6.1.2.1.1.1.0", ".1.3.6.1.2.1.1.1.0")]
    [InlineData(".1.3.6.1.2.1", ".1.3.6.1.2.1")]
    public void Oid_ShouldNormalizeLeadingDot(string input, string expected)
    {
        var oid = Oid.From(input);
        Assert.Equal(expected, oid.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("invalid.oid.string")]
    [InlineData(".1.3.6.a.2")]
    public void Oid_ShouldThrowArgumentException_WhenInvalidFormat(string input)
    {
        Assert.Throws<ArgumentException>(() => Oid.From(input));
    }

    [Fact]
    public void Oid_IsPrefixOf_ShouldReturnTrueForChildOid()
    {
        var parent = Oid.From(".1.3.6.1.2.1");
        var child = Oid.From(".1.3.6.1.2.1.1.1.0");

        Assert.True(parent.IsPrefixOf(child));
    }

    [Fact]
    public void Oid_IsPrefixOf_ShouldReturnFalseForUnrelatedOid()
    {
        var parent = Oid.From(".1.3.6.1.2.1");
        var unrelated = Oid.From(".1.3.6.1.4.1");

        Assert.False(parent.IsPrefixOf(unrelated));
    }
}