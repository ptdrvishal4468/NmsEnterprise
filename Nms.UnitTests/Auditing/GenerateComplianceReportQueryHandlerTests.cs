using System.Text;
using FluentAssertions;
using Moq;
using Nms.Application.Auditing.Dtos;
using Nms.Application.Auditing.Queries.GenerateComplianceReport;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reporting.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Auditing;

public class GenerateComplianceReportQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepoMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Mock<ICsvReportFormatter> _csvFormatterMock;
    private readonly GenerateComplianceReportQueryHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public GenerateComplianceReportQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _auditLogRepoMock = new Mock<IAuditLogRepository>();
        _tenantContextMock = new Mock<ITenantContext>();
        _csvFormatterMock = new Mock<ICsvReportFormatter>();

        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _unitOfWorkMock.Setup(u => u.AuditLogs).Returns(_auditLogRepoMock.Object);

        _handler = new GenerateComplianceReportQueryHandler(
            _unitOfWorkMock.Object,
            _tenantContextMock.Object,
            _csvFormatterMock.Object);
    }

    [Fact]
    public async Task Handle_JsonFormat_ReturnsComplianceReportDto()
    {
        // Arrange
        var logs = new List<AuditLog>
        {
            new AuditLog(_tenantId, Guid.NewGuid(), "Auth.Login", category: AuditCategory.Authentication, status: AuditStatus.Success),
            new AuditLog(_tenantId, Guid.NewGuid(), "Auth.Failed", category: AuditCategory.Authentication, status: AuditStatus.Failure),
            new AuditLog(_tenantId, Guid.NewGuid(), "Device.ConfigUpdate", category: AuditCategory.Configuration, status: AuditStatus.Success),
            new AuditLog(_tenantId, Guid.NewGuid(), "Role.Elevate", category: AuditCategory.Security, status: AuditStatus.Failure)
        };

        _auditLogRepoMock.Setup(r => r.SearchAuditLogsAsync(
            _tenantId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), null, null, null, null, null, null, null, 1, 1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync((logs, 4));

        var query = new GenerateComplianceReportQuery(Format: ReportFormat.Json);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ComplianceReportDto>();
        var dto = (ComplianceReportDto)result;
        dto.TotalAuditEvents.Should().Be(4);
        dto.TotalAuthenticationEvents.Should().Be(2);
        dto.FailedAuthenticationEvents.Should().Be(1);
        dto.TotalConfigurationChanges.Should().Be(1);
        dto.TotalSecurityViolations.Should().Be(2); // 1 Security category failure + 1 Auth failure
    }

    [Fact]
    public async Task Handle_CsvFormat_ReturnsReportExportResultDto()
    {
        // Arrange
        var logs = new List<AuditLog>
        {
            new AuditLog(_tenantId, Guid.NewGuid(), "Auth.Login", category: AuditCategory.Authentication, status: AuditStatus.Success)
        };

        _auditLogRepoMock.Setup(r => r.SearchAuditLogsAsync(
            _tenantId, It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), null, null, null, null, null, null, null, 1, 1000, It.IsAny<CancellationToken>()))
            .ReturnsAsync((logs, 1));

        var dummyBytes = Encoding.UTF8.GetBytes("Id,Action\n1,Auth.Login");
        _csvFormatterMock.Setup(f => f.FormatToCsv(It.IsAny<IEnumerable<AuditLogDto>>()))
            .Returns(dummyBytes);

        var query = new GenerateComplianceReportQuery(Format: ReportFormat.Csv);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ReportExportResultDto>();
        var export = (ReportExportResultDto)result;
        export.ContentType.Should().Be("text/csv");
        export.Data.Should().BeEquivalentTo(dummyBytes);
        export.TotalRecords.Should().Be(1);
    }
}