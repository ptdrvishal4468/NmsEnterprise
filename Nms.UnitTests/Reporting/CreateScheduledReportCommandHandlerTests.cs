using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Reporting.Commands.CreateScheduledReport;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Reporting;

public class CreateScheduledReportCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IScheduledReportRepository> _scheduledReportRepoMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly CreateScheduledReportCommandHandler _handler;

    public CreateScheduledReportCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _scheduledReportRepoMock = new Mock<IScheduledReportRepository>();
        _tenantContextMock = new Mock<ITenantContext>();

        _unitOfWorkMock.Setup(u => u.ScheduledReports).Returns(_scheduledReportRepoMock.Object);
        _tenantContextMock.Setup(t => t.TenantId).Returns(Guid.NewGuid());

        _handler = new CreateScheduledReportCommandHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_PersistsScheduledReportAndReturnsId()
    {
        // Arrange
        var command = new CreateScheduledReportCommand(
            "Weekly Device Health",
            ReportType.Health,
            ReportScheduleFrequency.Weekly,
            ReportFormat.Csv,
            "admin@enterprise.com");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _scheduledReportRepoMock.Verify(r => r.AddAsync(It.IsAny<ScheduledReport>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}