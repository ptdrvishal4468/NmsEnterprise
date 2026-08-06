namespace Nms.Infrastructure.Snmp.Exceptions;

public class SnmpAuthenticationException : SnmpException
{
    public SnmpAuthenticationException(string message) : base(message) { }
    public SnmpAuthenticationException(string message, Exception innerException) : base(message, innerException) { }
}