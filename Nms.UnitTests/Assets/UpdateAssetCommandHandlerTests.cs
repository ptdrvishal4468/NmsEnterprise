using Moq;
using Nms.Application.Assets.Commands.UpdateAsset;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Assets;

public class UpdateAssetCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateAssetCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_ExistingAsset_UpdatesPropertiesAndSaves()
    {
        var assetId = Guid.NewGuid();
        var existingAsset = new Asset(assetId, Guid.NewGuid(), "TAG-001", "Old Device Name");

        var assetRepoMock = new Mock<IAssetRepository>();
        assetRepoMock.Setup(r => r.GetByIdAsync(assetId, It.IsAny<CancellationToken>())).ReturnsAsync(existingAsset);

        _unitOfWorkMock.Setup(u => u.Assets).Returns(assetRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new UpdateAssetCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateAssetCommand(
            Id: assetId,
            Name: "New Device Name",
            Vendor: "Juniper",
            Model: "SRX300",
            WarrantyStatus: WarrantyStatus.Active);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("New Device Name", result.Name);
        Assert.Equal("Juniper", result.Vendor);
        Assert.Equal("SRX300", result.Model);

        assetRepoMock.Verify(r => r.Update(existingAsset), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistentAsset_ReturnsNull()
    {
        var assetRepoMock = new Mock<IAssetRepository>();
        assetRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Asset?)null);

        _unitOfWorkMock.Setup(u => u.Assets).Returns(assetRepoMock.Object);

        var handler = new UpdateAssetCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateAssetCommand(Id: Guid.NewGuid(), Name: "Missing Asset");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Null(result);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}