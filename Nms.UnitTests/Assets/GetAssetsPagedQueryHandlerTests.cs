using Moq;
using Nms.Application.Assets.Queries.GetAssetsPaged;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Assets;

public class GetAssetsPagedQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public GetAssetsPagedQueryHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    [Fact]
    public async Task Handle_WithSearchAndFiltering_ReturnsFilteredPagedResult()
    {
        var tenantId = Guid.NewGuid();
        var assets = new List<Asset>
        {
            new(Guid.NewGuid(), tenantId, "TAG-101", "Cisco Router 1", AssetLifecycleState.InService, vendor: "Cisco", department: "Engineering"),
            new(Guid.NewGuid(), tenantId, "TAG-102", "Cisco Switch 2", AssetLifecycleState.InStock, vendor: "Cisco", department: "Engineering"),
            new(Guid.NewGuid(), tenantId, "TAG-201", "Juniper Firewall", AssetLifecycleState.InService, vendor: "Juniper", department: "Security")
        };

        var assetRepoMock = new Mock<IAssetRepository>();
        assetRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(assets);

        _unitOfWorkMock.Setup(u => u.Assets).Returns(assetRepoMock.Object);

        var handler = new GetAssetsPagedQueryHandler(_unitOfWorkMock.Object);
        var query = new GetAssetsPagedQuery(
            PageNumber: 1,
            PageSize: 10,
            SearchTerm: "Cisco",
            LifecycleState: AssetLifecycleState.InService);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("TAG-101", result.Items[0].AssetTag);
        Assert.Equal(1, result.TotalCount);
    }
}