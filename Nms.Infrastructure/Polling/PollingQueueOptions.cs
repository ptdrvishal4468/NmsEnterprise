namespace Nms.Infrastructure.Polling;

public sealed class PollingQueueOptions
{
    public const string SectionName = "PollingQueue";

    public int Capacity { get; set; } = 10_000;
}