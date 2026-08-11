namespace Nms.Application.Interfaces.Services;

public static class InterfaceUtilizationCalculator
{
    public static double CalculateUtilization(
        long previousOctets,
        long currentOctets,
        double elapsedSeconds,
        long speedBps)
    {
        if (elapsedSeconds <= 0 || speedBps <= 0)
            return 0.0;

        long deltaOctets;
        if (currentOctets >= previousOctets)
        {
            deltaOctets = currentOctets - previousOctets;
        }
        else
        {
            // Handles 64-bit counter rollover
            deltaOctets = (long.MaxValue - previousOctets) + currentOctets;
        }

        if (deltaOctets < 0)
            return 0.0;

        double bitsTransferred = deltaOctets * 8.0;
        double bps = bitsTransferred / elapsedSeconds;
        double utilization = (bps / speedBps) * 100.0;

        return Math.Clamp(Math.Round(utilization, 2), 0.0, 100.0);
    }
}