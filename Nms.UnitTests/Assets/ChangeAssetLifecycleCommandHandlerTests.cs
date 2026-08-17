using Moq;
using Nms.Application.Assets.Commands.ChangeAssetLifecycle;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Assets;

public class ChangeAssetLifecycleCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public ChangeAssetLifecycleCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_ExistingAsset_AppliesLifecycleTransition()
    {
        var assetId = Guid.NewGuid();
        var existingAsset = new Asset(assetId, Guid.NewGuid(), "TAG-005", "Core Switch", AssetLifecycleState.InStock);

        var assetRepoMock = new Mock<IAssetRepository>();
        assetRepoMock.Setup(r => r.GetByIdAsync(assetId, It.IsAny<CancellationToken>())).ReturnsAsync(existingAsset);

        _unitOfWorkMock.Setup(u => u.Assets).Returns(assetRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new ChangeAssetLifecycleCommandHandler(_unitOfWorkMock.Object);
        var command = new ChangeAssetLifecycleCommand(assetId, AssetLifecycleState.InService, "Put into production.");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(AssetLifecycleState.InService, result.LifecycleState);
        Assert.Equal("Put into production.", result.LifecycleNotes);

        assetRepoMock.Verify(r => r.Update(existingAsset), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}