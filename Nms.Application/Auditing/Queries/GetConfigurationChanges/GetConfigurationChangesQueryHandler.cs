using MediatR;
using Nms.Application.Auditing.Dtos;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Interfaces;

namespace Nms.Application.Auditing.Queries.GetConfigurationChanges;

public class GetConfigurationChangesQueryHandler : IRequestHandler<GetConfigurationChangesQuery, IReadOnlyList<ConfigurationChangeDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetConfigurationChangesQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<IReadOnlyList<ConfigurationChangeDto>> Handle(GetConfigurationChangesQuery request, CancellationToken cancellationToken)
    {
        var maxCount = request.MaxCount switch
        {
            < 1 => 50,
            > 200 => 200,
            _ => request.MaxCount
        };

        var changes = await _unitOfWork.AuditLogs.GetConfigurationChangesAsync(
            tenantId: _tenantContext.TenantId,
            fromUtc: request.FromUtc,
            toUtc: request.ToUtc,
            entityName: request.EntityName,
            maxCount: maxCount,
            cancellationToken: cancellationToken);

        return changes.Select(c => new ConfigurationChangeDto
        {
            AuditLogId = c.Id,
            UserId = c.UserId,
            Username = c.Username,
            Action = c.Action,
            EntityName = c.EntityName,
            EntityId = c.EntityId,
            OldValuesJson = c.OldValuesJson,
            NewValuesJson = c.NewValuesJson,
            TimestampUtc = c.TimestampUtc
        }).ToList();
    }
}