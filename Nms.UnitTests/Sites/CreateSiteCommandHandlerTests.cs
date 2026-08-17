using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Sites.Commands.CreateSite;
using Nms.Application.Sites.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Sites;

public class CreateSiteCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISiteRepository> _mockSiteRepo;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly CreateSiteCommandHandler _handler;

    public CreateSiteCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSiteRepo = new Mock<ISiteRepository>();
        _mockTenantContext = new Mock<ITenantContext>();

        _mockUnitOfWork.Setup(u => u.Sites).Returns(_mockSiteRepo.Object);
        _mockTenantContext.Setup(t => t.TenantId).Returns(Guid.NewGuid());

        _handler = new CreateSiteCommandHandler(_mockUnitOfWork.Object, _mockTenantContext.Object);
    }

    [Fact]
    public async Task Handle_WhenCodeIsUnique_CreatesAndReturnsSiteDto()
    {
        // Arrange
        var dto = new CreateSiteDto("HQ Campus", "HQ-01", null, null, "Austin", "TX", "78701", "USA", null, null, null);
        var command = new CreateSiteCommand(dto);

        _mockSiteRepo.Setup(r => r.CodeExistsAsync(dto.Code, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        Assert.Equal("HQ-01", result.Code);
        _mockSiteRepo.Verify(r => r.AddAsync(It.IsAny<Site>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenCodeAlreadyExists_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new CreateSiteDto("HQ Campus", "DUPLICATE", null, null, null, null, null, null, null, null, null);
        var command = new CreateSiteCommand(dto);

        _mockSiteRepo.Setup(r => r.CodeExistsAsync(dto.Code, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        _mockSiteRepo.Verify(r => r.AddAsync(It.IsAny<Site>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}