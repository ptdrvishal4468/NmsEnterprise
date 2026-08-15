using FluentAssertions;
using Moq;
using Nms.Application.Auditing.Queries.SearchAuditLogs;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Auditing;

public class SearchAuditLogsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepoMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly SearchAuditLogsQueryHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public SearchAuditLogsQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _auditLogRepoMock = new Mock<IAuditLogRepository>();
        _tenantContextMock = new Mock<ITenantContext>();

        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _unitOfWorkMock.Setup(u => u.AuditLogs).Returns(_auditLogRepoMock.Object);

        _handler = new SearchAuditLogsQueryHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);
    }

    [Fact]
    public async Task Handle_WithFilters_ReturnsPagedResult()
    {
        // Arrange
        var logs = new List<AuditLog>
        {
            new AuditLog(_tenantId, Guid.NewGuid(), "Role.Update", category: AuditCategory.RoleManagement),
            new AuditLog(_tenantId, Guid.NewGuid(), "User.Create", category: AuditCategory.UserManagement)
        };

        _auditLogRepoMock.Setup(r => r.SearchAuditLogsAsync(
            _tenantId, null, null, null, null, null, null, null, null, null, 1, 20, It.IsAny<CancellationToken>()))
            .ReturnsAsync((logs, 2));

        var query = new SearchAuditLogsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(2);
        result.Items.Should().HaveCount(2);
        result.Items[0].Action.Should().Be("Role.Update");
        result.Items[0].Category.Should().Be(AuditCategory.RoleManagement.ToString());
    }
}