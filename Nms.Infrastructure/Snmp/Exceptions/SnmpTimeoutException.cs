namespace Nms.Infrastructure.Snmp.Exceptions;

public class SnmpTimeoutException : SnmpException
{
    public SnmpTimeoutException(string host, int port, TimeSpan timeout)
        : base($"SNMP request to {host}:{port} timed out after {timeout.TotalMilliseconds}ms.") { }
}