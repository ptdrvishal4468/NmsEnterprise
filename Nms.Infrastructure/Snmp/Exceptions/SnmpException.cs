namespace Nms.Infrastructure.Snmp.Exceptions;

public class SnmpException : Exception
{
    public SnmpException(string message) : base(message) { }
    public SnmpException(string message, Exception innerException) : base(message, innerException) { }
}