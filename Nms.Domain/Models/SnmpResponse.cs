using System.Collections.Generic;

namespace Nms.Domain.Models;

public sealed record SnmpResponse
{
    public bool IsSuccess { get; }
    public string? ErrorMessage { get; }
    public IReadOnlyList<SnmpDataValue> DataValues { get; }
    public double LatencyMs { get; }
    public DateTime TimestampUtc { get; }

    private SnmpResponse(bool isSuccess, string? errorMessage, IReadOnlyList<SnmpDataValue> dataValues, double latencyMs)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
        DataValues = dataValues ?? Array.Empty<SnmpDataValue>();
        LatencyMs = latencyMs;
        TimestampUtc = DateTime.UtcNow;
    }

    public static SnmpResponse Success(IReadOnlyList<SnmpDataValue> dataValues, double latencyMs)
        => new(true, null, dataValues, latencyMs);

    public static SnmpResponse Failure(string errorMessage, double latencyMs = 0)
        => new(false, errorMessage, Array.Empty<SnmpDataValue>(), latencyMs);
}