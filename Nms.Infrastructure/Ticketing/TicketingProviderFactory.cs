using Microsoft.Extensions.DependencyInjection;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;
using Nms.Infrastructure.Ticketing.Jira;
using Nms.Infrastructure.Ticketing.ServiceNow;

namespace Nms.Infrastructure.Ticketing;

public class TicketingProviderFactory : ITicketingProviderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public TicketingProviderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITicketingProvider GetProvider(TicketingProviderType providerType)
    {
        return providerType switch
        {
            TicketingProviderType.ServiceNow => _serviceProvider.GetRequiredService<ServiceNowTicketingProvider>(),
            TicketingProviderType.Jira => _serviceProvider.GetRequiredService<JiraTicketingProvider>(),
            _ => throw mechanicalArgumentOutOfRangeException(providerType)
        };
    }

    private static ArgumentOutOfRangeException mechanicalArgumentOutOfRangeException(TicketingProviderType providerType) =>
        new(nameof(providerType), $"Unsupported ticketing provider: {providerType}");
}