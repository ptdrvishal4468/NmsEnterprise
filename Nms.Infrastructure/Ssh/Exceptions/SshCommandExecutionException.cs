namespace Nms.Infrastructure.Ssh.Exceptions;

public class SshCommandExecutionException : Exception
{
    public SshCommandExecutionException(string message, Exception innerException) : base(message, innerException) { }
}