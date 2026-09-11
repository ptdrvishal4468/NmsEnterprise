using Nms.Domain.Models;

namespace Nms.Application.Common.Interfaces;

public interface ISshClient : IAsyncDisposable
{
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken);
    Task<SshExecutionResult> ExecuteCommandAsync(string command, TimeSpan timeout, CancellationToken cancellationToken);
}