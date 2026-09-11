using MediatR;
using Moq;
using Nms.Application.Reporting.Commands.ExecuteScheduledReport;
using Nms.Application.Reporting.Dtos;
using Nms.Application.Reporting.Queries.GenerateDeviceReport;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Reporting;

public class ExecuteScheduledReportCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IScheduledReportRepository> _scheduledReportRepoMock;
    private readonly Mock<IScheduledReportExecutionLogRepository> _executionLogRepoMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ExecuteScheduledReportCommandHandler _handler;

    public ExecuteScheduledReportCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _scheduledReportRepoMock = new Mock<IScheduledReportRepository>();
        _executionLogRepoMock = new Mock<IScheduledReportExecutionLogRepository>();
        _mediatorMock = new Mock<IMediator>();

        _unitOfWorkMock.Setup(u => u.ScheduledReports).Returns(_scheduledReportRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.ScheduledReportExecutionLogs).Returns(_executionLogRepoMock.Object);

        _handler = new ExecuteScheduledReportCommandHandler(_unitOfWorkMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_SuccessfulReportGeneration_LogsExecutionSuccess()
    {
        // Arrange
        var reportId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var scheduledReport = new ScheduledReport(
            reportId,
            tenantId,
            "Daily Device Report",
            ReportType.Device,
            ReportScheduleFrequency.Daily,
            ReportFormat.Json,
            "ops@enterprise.com");

        _scheduledReportRepoMock.Setup(r => r.GetByIdAsync(reportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(scheduledReport);

        _mediatorMock.Setup(m => m.Send(It.IsAny<GenerateDeviceReportQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ReportExportResultDto { TotalRecords = 12, Data = Array.Empty<byte>() });

        var command = new ExecuteScheduledReportCommand(reportId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ReportExecutionStatus.Success, result.Status);
        Assert.Equal(12, result.RecordCount);
        _executionLogRepoMock.Verify(l => l.AddAsync(It.IsAny<ScheduledReportExecutionLog>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}