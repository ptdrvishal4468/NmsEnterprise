namespace Nms.Application.Common.Interfaces;

public interface ISyslogReceiver
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}