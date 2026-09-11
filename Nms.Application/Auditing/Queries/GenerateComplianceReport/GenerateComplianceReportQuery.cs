using MediatR;
using Nms.Domain.Enums;

namespace Nms.Application.Auditing.Queries.GenerateComplianceReport;

public record GenerateComplianceReportQuery(
    DateTime? FromUtc = null,
    DateTime? ToUtc = null,
    ReportFormat Format = ReportFormat.Json
) : IRequest<object>;