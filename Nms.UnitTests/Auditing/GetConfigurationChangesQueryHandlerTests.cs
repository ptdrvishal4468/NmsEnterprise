using FluentAssertions;
using Moq;
using Nms.Application.Auditing.Queries.GetConfigurationChanges;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Auditing;

public class GetConfigurationChangesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAuditLogRepository> _auditLogRepoMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly GetConfigurationChangesQueryHandler _handler;
    private readonly Guid _tenantId = Guid.NewGuid();

    public GetConfigurationChangesQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _auditLogRepoMock = new Mock<IAuditLogRepository>();
        _tenantContextMock = new Mock<ITenantContext>();

        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _unitOfWorkMock.Setup(u => u.AuditLogs).Returns(_auditLogRepoMock.Object);

        _handler = new GetConfigurationChangesQueryHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsConfigurationChanges()
    {
        // Arrange
        var logs = new List<AuditLog>
        {
            new AuditLog(_tenantId, Guid.NewGuid(), "AlertRule.Update",
                oldValuesJson: "{\"Threshold\":80}",
                newValuesJson: "{\"Threshold\":90}",
                entityName: "AlertRule",
                category: AuditCategory.Configuration)
        };

        _auditLogRepoMock.Setup(r => r.GetConfigurationChangesAsync(
            _tenantId, null, null, "AlertRule", 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(logs);

        var query = new GetConfigurationChangesQuery(EntityName: "AlertRule", MaxCount: 50);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result[0].Action.Should().Be("AlertRule.Update");
        result[0].OldValuesJson.Should().Be("{\"Threshold\":80}");
        result[0].NewValuesJson.Should().Be("{\"Threshold\":90}");
    }
}