namespace Nms.Infrastructure.Ssh.Exceptions;

public class SshAuthenticationException : Exception
{
    public SshAuthenticationException(string message) : base(message) { }
    public SshAuthenticationException(string message, Exception innerException) : base(message, innerException) { }
}