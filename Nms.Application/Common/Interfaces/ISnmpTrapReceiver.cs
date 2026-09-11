namespace Nms.Application.Common.Interfaces;

public interface ISnmpTrapReceiver
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}