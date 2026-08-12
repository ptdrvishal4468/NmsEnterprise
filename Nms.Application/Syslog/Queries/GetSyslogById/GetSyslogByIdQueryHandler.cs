using MediatR;
using Nms.Application.Syslog.Dtos;
using Nms.Domain.Interfaces;

namespace Nms.Application.Syslog.Queries.GetSyslogById;

public class GetSyslogByIdQueryHandler : IRequestHandler<GetSyslogByIdQuery, SyslogMessageDto?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSyslogByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SyslogMessageDto?> Handle(GetSyslogByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Syslogs.GetByIdAsync(request.Id, cancellationToken);
        if (item == null || item.TenantId != request.TenantId)
        {
            return null;
        }

        return new SyslogMessageDto
        {
            Id = item.Id,
            TenantId = item.TenantId,
            DeviceId = item.DeviceId,
            Facility = item.Facility,
            Severity = item.Severity,
            FacilityName = item.FacilityName,
            SeverityName = item.SeverityName,
            TimestampUtc = item.TimestampUtc,
            Hostname = item.Hostname,
            AppTag = item.AppTag,
            ProcessId = item.ProcessId,
            MessageId = item.MessageId,
            Message = item.Message,
            RawMessage = item.RawMessage,
            SourceIpAddress = item.SourceIpAddress,
            IsMalformed = item.IsMalformed,
            CreatedAtUtc = item.CreatedAtUtc
        };
    }
}