using Nms.Domain.Models;
using Nms.Domain.ValueObjects;

namespace Nms.Application.Common.Interfaces;

public interface ISnmpClientFactory
{
    ISnmpClient CreateV2cClient(string hostOrIp, int port, SnmpV2Credentials credentials, TimeSpan? timeout = null);
    ISnmpClient CreateV3Client(string hostOrIp, int port, SnmpV3Credentials credentials, TimeSpan? timeout = null);
    ISnmpClient CreateClient(ConnectionParameters connectionParameters);
}