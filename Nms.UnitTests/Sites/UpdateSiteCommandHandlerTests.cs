using Moq;
using Nms.Application.Sites.Commands.UpdateSite;
using Nms.Application.Sites.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Sites;

public class UpdateSiteCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISiteRepository> _mockSiteRepo;
    private readonly UpdateSiteCommandHandler _handler;

    public UpdateSiteCommandHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSiteRepo = new Mock<ISiteRepository>();
        _mockUnitOfWork.Setup(u => u.Sites).Returns(_mockSiteRepo.Object);

        _handler = new UpdateSiteCommandHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WhenSiteExists_UpdatesAndReturnsDto()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var existingSite = new Site(siteId, tenantId, "Old Name", "OLD-CODE");

        var updateDto = new UpdateSiteDto("Updated Name", "NEW-CODE", "Desc", null, "Berlin", null, null, "Germany", null, null, null, true);
        var command = new UpdateSiteCommand(siteId, updateDto);

        _mockSiteRepo.Setup(r => r.GetByIdAsync(siteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSite);
        _mockSiteRepo.Setup(r => r.CodeExistsAsync(updateDto.Code, siteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("NEW-CODE", result.Code);
        _mockSiteRepo.Verify(r => r.Update(existingSite), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenSiteNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var updateDto = new UpdateSiteDto("Name", "CODE", null, null, null, null, null, null, null, null, null, true);
        var command = new UpdateSiteCommand(siteId, updateDto);

        _mockSiteRepo.Setup(r => r.GetByIdAsync(siteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Site?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}