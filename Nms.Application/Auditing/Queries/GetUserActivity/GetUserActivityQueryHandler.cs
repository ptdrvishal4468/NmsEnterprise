using MediatR;
using Nms.Application.Auditing.Dtos;
using Nms.Application.Common.Interfaces;
using Nms.Application.Common.Models;
using Nms.Domain.Interfaces;

namespace Nms.Application.Auditing.Queries.GetUserActivity;

public class GetUserActivityQueryHandler : IRequestHandler<GetUserActivityQuery, PagedResult<AuditLogDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;

    public GetUserActivityQueryHandler(IUnitOfWork unitOfWork, ITenantContext tenantContext)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
    }

    public async Task<PagedResult<AuditLogDto>> Handle(GetUserActivityQuery request, CancellationToken cancellationToken)
    {
        var pageIndex = request.PageIndex < 1 ? 1 : request.PageIndex;
        var pageSize = request.PageSize switch
        {
            < 1 => 20,
            > 100 => 100,
            _ => request.PageSize
        };

        var (items, totalCount) = await _unitOfWork.AuditLogs.GetUserActivityAsync(
            tenantId: _tenantContext.TenantId,
            userId: request.UserId,
            fromUtc: request.FromUtc,
            toUtc: request.ToUtc,
            pageIndex: pageIndex,
            pageSize: pageSize,
            cancellationToken: cancellationToken);

        var dtos = items.Select(a => new AuditLogDto
        {
            Id = a.Id,
            TenantId = a.TenantId,
            UserId = a.UserId,
            Username = a.Username,
            Action = a.Action,
            EntityName = a.EntityName,
            EntityId = a.EntityId,
            Category = a.Category.ToString(),
            Status = a.Status.ToString(),
            IpAddress = a.IpAddress,
            Details = a.Details,
            OldValuesJson = a.OldValuesJson,
            NewValuesJson = a.NewValuesJson,
            TimestampUtc = a.TimestampUtc
        }).ToList();

        return new PagedResult<AuditLogDto>(dtos, totalCount, pageIndex, pageSize);
    }
}