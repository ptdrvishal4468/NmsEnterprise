using MediatR;
using Nms.Application.Common.Models;
using Nms.Application.Syslog.Dtos;
using Nms.Application.Syslog.Queries.GetSyslogsPaged;
using Nms.Domain.Interfaces;

namespace Nms.Application.Syslog.Queries.GetSyslogsPaged;

public class GetSyslogsPagedQueryHandler : IRequestHandler<GetSyslogsPagedQuery, PagedResult<SyslogMessageDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSyslogsPagedQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<SyslogMessageDto>> Handle(GetSyslogsPagedQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _unitOfWork.Syslogs.GetSyslogsPagedAsync(
            tenantId: request.TenantId,
            deviceId: request.DeviceId,
            severity: request.Severity,
            facility: request.Facility,
            sourceIp: request.SourceIp,
            searchKeyword: request.SearchKeyword,
            startDateUtc: request.StartDateUtc,
            endDateUtc: request.EndDateUtc,
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            cancellationToken: cancellationToken);

        var dtos = items.Select(x => new SyslogMessageDto
        {
            Id = x.Id,
            TenantId = x.TenantId,
            DeviceId = x.DeviceId,
            Facility = x.Facility,
            Severity = x.Severity,
            FacilityName = x.FacilityName,
            SeverityName = x.SeverityName,
            TimestampUtc = x.TimestampUtc,
            Hostname = x.Hostname,
            AppTag = x.AppTag,
            ProcessId = x.ProcessId,
            MessageId = x.MessageId,
            Message = x.Message,
            RawMessage = x.RawMessage,
            SourceIpAddress = x.SourceIpAddress,
            IsMalformed = x.IsMalformed,
            CreatedAtUtc = x.CreatedAtUtc
        }).ToList();

        return new PagedResult<SyslogMessageDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}