using Nms.Domain.Enums;

namespace Nms.Application.Common.Interfaces;

public interface ITicketingProviderFactory
{
    ITicketingProvider GetProvider(TicketingProviderType providerType);
}