using Nms.Application.Firmware.Services;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Firmware;

public class FirmwareVersionComparatorTests
{
    private readonly FirmwareVersionComparator _comparator = new();

    [Theory]
    [InlineData("1.0.0", "1.0.0", 0)]
    [InlineData("1.0.0", "1.1.0", -1)]
    [InlineData("2.0.0", "1.9.9", 1)]
    [InlineData("15.2.4", "15.2.4", 0)]
    [InlineData("15.2.4M", "15.2.4M1", -1)]
    [InlineData("21.4R1.12", "21.4R1.12", 0)]
    public void Compare_ShouldEvaluateVersionRelationshipsCorrectly(string v1, string v2, int expectedResultSign)
    {
        var result = _comparator.Compare(v1, v2);

        if (expectedResultSign == 0)
            Assert.Equal(0, result);
        else if (expectedResultSign < 0)
            Assert.True(result < 0, $"Expected {v1} < {v2}");
        else
            Assert.True(result > 0, $"Expected {v1} > {v2}");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void EvaluateCompliance_ShouldReturnCompliant_WhenVersionsMatch()
    {
        var result = _comparator.EvaluateCompliance("15.2(4)M", "15.2(4)M");
        Assert.Equal(FirmwareComplianceStatus.Compliant, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void EvaluateCompliance_ShouldReturnNonCompliant_WhenVersionsMismatch()
    {
        var result = _comparator.EvaluateCompliance("15.1.0", "15.2.0");
        Assert.Equal(FirmwareComplianceStatus.NonCompliant, result);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void EvaluateCompliance_ShouldReturnUnknown_WhenTargetVersionIsNull()
    {
        var result = _comparator.EvaluateCompliance("15.1.0", null);
        Assert.Equal(FirmwareComplianceStatus.Unknown, result);
    }
}