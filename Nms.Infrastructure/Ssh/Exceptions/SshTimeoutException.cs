namespace Nms.Infrastructure.Ssh.Exceptions;

public class SshTimeoutException : Exception
{
    public SshTimeoutException(string message) : base(message) { }
}