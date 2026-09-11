using MediatR;
using Nms.Application.Syslog.Dtos;

namespace Nms.Application.Syslog.Queries.GetSyslogById;

public record GetSyslogByIdQuery(Guid Id, Guid TenantId) : IRequest<SyslogMessageDto?>;