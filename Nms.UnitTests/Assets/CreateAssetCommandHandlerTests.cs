using Moq;
using Nms.Application.Assets.Commands.CreateAsset;
using Nms.Application.Common.Interfaces;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Assets;

public class CreateAssetCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITenantContext> _tenantContextMock;
    private readonly Guid _testTenantId = Guid.NewGuid();

    public CreateAssetCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _tenantContextMock = new Mock<ITenantContext>();
        _tenantContextMock.Setup(t => t.TenantId).Returns(_testTenantId);
    }

    [Fact]
    public async Task Handle_UniqueAssetTag_CreatesAndPersistsAsset()
    {
        var assetRepoMock = new Mock<IAssetRepository>();
        assetRepoMock
            .Setup(r => r.AssetTagExistsAsync(It.IsAny<string>(), null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _unitOfWorkMock.Setup(u => u.Assets).Returns(assetRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateAssetCommandHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);
        var command = new CreateAssetCommand(
            AssetTag: "TAG-TEST-1",
            Name: "Edge Router",
            LifecycleState: AssetLifecycleState.InStock,
            PurchasePrice: 1200.00m);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("TAG-TEST-1", result.AssetTag);
        Assert.Equal("Edge Router", result.Name);
        Assert.Equal(_testTenantId, result.TenantId);

        assetRepoMock.Verify(r => r.AddAsync(It.IsAny<Asset>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateAssetTag_ThrowsInvalidOperationException()
    {
        var assetRepoMock = new Mock<IAssetRepository>();
        assetRepoMock
            .Setup(r => r.AssetTagExistsAsync("TAG-DUPLICATE", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _unitOfWorkMock.Setup(u => u.Assets).Returns(assetRepoMock.Object);

        var handler = new CreateAssetCommandHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);
        var command = new CreateAssetCommand(AssetTag: "TAG-DUPLICATE", Name: "Duplicate Node");

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
        assetRepoMock.Verify(r => r.AddAsync(It.IsAny<Asset>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}