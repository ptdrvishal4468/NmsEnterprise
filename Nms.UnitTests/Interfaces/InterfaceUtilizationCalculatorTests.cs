using Nms.Application.Interfaces.Services;
using Xunit;

namespace Nms.UnitTests.Interfaces;

public class InterfaceUtilizationCalculatorTests
{
    [Fact]
    public void CalculateUtilization_ValidOctetsAndElapsed_ReturnsCorrectPercentage()
    {
        // 1,250,000 bytes = 10,000,000 bits over 10 seconds = 1,000,000 bps
        // On a 10 Mbps (10,000,000 bps) interface -> 10.00% utilization
        long previousOctets = 100_000_000;
        long currentOctets = 101_250_000;
        double elapsedSeconds = 10.0;
        long speedBps = 10_000_000;

        double result = InterfaceUtilizationCalculator.CalculateUtilization(
            previousOctets, currentOctets, elapsedSeconds, speedBps);

        Assert.Equal(10.0, result);
    }

    [Theory]
    [InlineData(0, 100)]
    [InlineData(10, 0)]
    [InlineData(-5, 1000)]
    public void CalculateUtilization_InvalidElapsedOrSpeed_ReturnsZero(double elapsedSeconds, long speedBps)
    {
        double result = InterfaceUtilizationCalculator.CalculateUtilization(
            100, 200, elapsedSeconds, speedBps);

        Assert.Equal(0.0, result);
    }

    [Fact]
    public void CalculateUtilization_CounterRollover_CalculatesCorrectly()
    {
        long previousOctets = long.MaxValue - 1_000_000;
        long currentOctets = 250_000; // Rolled over
        double elapsedSeconds = 10.0;
        long speedBps = 10_000_000;

        double result = InterfaceUtilizationCalculator.CalculateUtilization(
            previousOctets, currentOctets, elapsedSeconds, speedBps);

        Assert.True(result > 0.0);
        Assert.True(result <= 100.0);
    }
}