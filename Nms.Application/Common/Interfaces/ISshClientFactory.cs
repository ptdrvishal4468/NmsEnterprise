using Nms.Domain.Models;

namespace Nms.Application.Common.Interfaces;

public interface ISshClientFactory
{
    ISshClient CreateClient(string host, int port, SshCredentials credentials, TimeSpan timeout);
}