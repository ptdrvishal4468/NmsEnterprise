using FluentAssertions;
using Moq;
using Nms.Application.Auditing.Commands.RecordAuditLog;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Auditing;

public class RecordAuditLogCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepoMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly RecordAuditLogCommandHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public RecordAuditLogCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _auditLogRepoMock = new Mock<IAuditLogRepository>();
        _tenantContextMock = new Mock<ITenantContext>();

        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _unitOfWorkMock.Setup(u => u.AuditLogs).Returns(_auditLogRepoMock.Object);

        _handler = new RecordAuditLogCommandHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_PersistsAuditLogAndReturnsId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new RecordAuditLogCommand(
            UserId: userId,
            Username: "admin_user",
            Action: "Device.Create",
            EntityName: "Device",
            EntityId: Guid.NewGuid().ToString(),
            Category: AuditCategory.DeviceManagement,
            Status: AuditStatus.Success,
            IpAddress: "192.168.1.100",
            Details: "Created device router-core-01",
            OldValuesJson: null,
            NewValuesJson: "{\"Name\":\"router-core-01\"}"
        );

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _auditLogRepoMock.Verify(r => r.AddAsync(It.Is<AuditLog>(a =>
            a.TenantId == _tenantId &&
            a.UserId == userId &&
            a.Action == "Device.Create" &&
            a.Category == AuditCategory.DeviceManagement &&
            a.Status == AuditStatus.Success &&
            a.IpAddress == "192.168.1.100"
        ), It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}