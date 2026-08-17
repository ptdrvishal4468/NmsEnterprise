using MediatR;
using Nms.Application.Common.Interfaces;
using Nms.Application.Ticketing.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Ticketing.Commands.CreateTicket;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly ITicketingProviderFactory _providerFactory;

    public CreateTicketCommandHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        ITicketingProviderFactory providerFactory)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _providerFactory = providerFactory;
    }

    public async Task<TicketDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            TenantId = _tenantContext.TenantId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            Status = TicketStatus.Open,
            ProviderType = request.ProviderType,
            DeviceId = request.DeviceId,
            AlertId = request.AlertId,
            CustomerId = request.CustomerId,
            MetadataJson = request.MetadataJson
        };

        if (request.DispatchToProvider)
        {
            var provider = _providerFactory.GetProvider(request.ProviderType);
            try
            {
                var (externalId, externalKey, externalUrl) = await provider.CreateTicketAsync(ticket, cancellationToken);
                ticket.SetExternalDetails(externalId, externalKey, "New", externalUrl);

                ticket.SyncLogs.Add(new TicketSyncLog
                {
                    TenantId = _tenantContext.TenantId,
                    TicketId = ticket.Id,
                    ProviderType = request.ProviderType,
                    Direction = TicketSyncDirection.Outbound,
                    Status = TicketSyncStatus.Success,
                    RequestPayload = $"Create Ticket: {ticket.Title}",
                    ResponsePayload = $"ExternalId: {externalId}, Key: {externalKey}"
                });
            }
            catch (Exception ex)
            {
                ticket.SyncLogs.Add(new TicketSyncLog
                {
                    TenantId = _tenantContext.TenantId,
                    TicketId = ticket.Id,
                    ProviderType = request.ProviderType,
                    Direction = TicketSyncDirection.Outbound,
                    Status = TicketSyncStatus.Failed,
                    RequestPayload = $"Create Ticket: {ticket.Title}",
                    ErrorMessage = ex.Message
                });
            }
        }

        await _unitOfWork.Tickets.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new TicketDto(
            ticket.Id,
            ticket.TenantId,
            ticket.Title,
            ticket.Description,
            ticket.Priority,
            ticket.Status,
            ticket.ProviderType,
            ticket.ExternalTicketId,
            ticket.ExternalTicketKey,
            ticket.ExternalStatus,
            ticket.ExternalUrl,
            ticket.LastSyncedAtUtc,
            ticket.DeviceId,
            ticket.AlertId,
            ticket.CustomerId,
            ticket.MetadataJson,
            ticket.CreatedAtUtc,
            ticket.LastModifiedAtUtc);
    }
}
