using MediatR;
using Nms.Application.Auditing.Dtos;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;

namespace Nms.Application.Auditing.Queries.GenerateComplianceReport;

public class GenerateComplianceReportQueryHandler : IRequestHandler<GenerateComplianceReportQuery, object>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantContext _tenantContext;
    private readonly ICsvReportFormatter _csvFormatter;

    public GenerateComplianceReportQueryHandler(
        IUnitOfWork unitOfWork,
        ITenantContext tenantContext,
        ICsvReportFormatter csvFormatter)
    {
        _unitOfWork = unitOfWork;
        _tenantContext = tenantContext;
        _csvFormatter = csvFormatter;
    }

    public async Task<object> Handle(GenerateComplianceReportQuery request, CancellationToken cancellationToken)
    {
        var fromUtc = request.FromUtc ?? DateTime.UtcNow.AddDays(-30);
        var toUtc = request.ToUtc ?? DateTime.UtcNow;

        var (allLogs, totalCount) = await _unitOfWork.AuditLogs.SearchAuditLogsAsync(
            tenantId: _tenantContext.TenantId,
            fromUtc: fromUtc,
            toUtc: toUtc,
            userId: null,
            action: null,
            category: null,
            status: null,
            entityName: null,
            entityId: null,
            searchTerm: null,
            pageIndex: 1,
            pageSize: 1000,
            cancellationToken: cancellationToken);

        var logDtos = allLogs.Select(a => new AuditLogDto
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

        var authEvents = logDtos.Where(l => l.Category == AuditCategory.Authentication.ToString()).ToList();
        var failedAuthEvents = authEvents.Where(l => l.Status == AuditStatus.Failure.ToString()).ToList();
        var configChanges = logDtos.Where(l => l.Category == AuditCategory.Configuration.ToString()).ToList();
        var secViolations = logDtos.Where(l => l.Category == AuditCategory.Security.ToString() || l.Status == AuditStatus.Failure.ToString()).ToList();

        var complianceReport = new ComplianceReportDto
        {
            GeneratedAtUtc = DateTime.UtcNow,
            FromUtc = fromUtc,
            ToUtc = toUtc,
            TotalAuditEvents = totalCount,
            TotalAuthenticationEvents = authEvents.Count,
            FailedAuthenticationEvents = failedAuthEvents.Count,
            TotalConfigurationChanges = configChanges.Count,
            TotalSecurityViolations = secViolations.Count,
            TopFailedOperations = secViolations.Take(10).ToList(),
            CriticalEvents = logDtos.Where(l => l.Category == AuditCategory.Security.ToString() || l.Category == AuditCategory.RoleManagement.ToString()).Take(20).ToList()
        };

        if (request.Format == ReportFormat.Csv)
        {
            var csvBytes = _csvFormatter.FormatToCsv(logDtos);
            var fileName = $"Compliance_Report_{_tenantContext.TenantId}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
            return new ReportExportResultDto
            {
                FileName = fileName,
                ContentType = "text/csv",
                Data = csvBytes,
                TotalRecords = logDtos.Count
            };
        }

        return complianceReport;
    }
}