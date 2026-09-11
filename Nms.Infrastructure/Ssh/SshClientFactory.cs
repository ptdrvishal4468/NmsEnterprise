using Nms.Application.Common.Interfaces;
using Nms.Domain.Models;

namespace Nms.Infrastructure.Ssh;

public sealed class SshClientFactory : ISshClientFactory
{
    public ISshClient CreateClient(string host, int port, SshCredentials credentials, TimeSpan timeout)
    {
        return new SshClientWrapper(host, port, credentials, timeout);
    }
}