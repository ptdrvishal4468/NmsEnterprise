using Nms.Domain.Enums;

namespace Nms.Application.Common.Interfaces;

public interface IConnectionAdapterFactory
{
    IConnectionAdapter GetAdapter(NetworkProtocol protocol);
}