using FluentAssertions;
using Moq;
using Nms.Application.Auditing.Queries.GetUserActivity;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Auditing;

public class GetUserActivityQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepoMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly GetUserActivityQueryHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public GetUserActivityQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _auditLogRepoMock = new Mock<IAuditLogRepository>();
        _tenantContextMock = new Mock<ITenantContext>();

        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _unitOfWorkMock.Setup(u => u.AuditLogs).Returns(_auditLogRepoMock.Object);

        _handler = new GetUserActivityQueryHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);
    }

    [Fact]
    public async Task Handle_UserSpecificActivity_ReturnsPagedResult()
    {
        // Arrange
        var targetUser = Guid.NewGuid();
        var logs = new List<AuditLog>
        {
            new AuditLog(_tenantId, targetUser, "Auth.Login", category: AuditCategory.Authentication),
            new AuditLog(_tenantId, targetUser, "Device.Update", category: AuditCategory.DeviceManagement)
        };

        _auditLogRepoMock.Setup(r => r.GetUserActivityAsync(
            _tenantId, targetUser, null, null, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((logs, 2));

        var query = new GetUserActivityQuery(UserId: targetUser);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items.All(i => i.UserId == targetUser).Should().BeTrue();
    }
}