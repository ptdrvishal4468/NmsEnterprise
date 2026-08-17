using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Locations.Buildings.Commands.CreateBuilding;
using Nms.Application.Locations.Buildings.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Locations;

public class BuildingHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISiteRepository> _mockSiteRepo;
    private readonly Mock<IBuildingRepository> _mockBuildingRepo;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly CreateBuildingCommandHandler _createHandler;

    public BuildingHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSiteRepo = new Mock<ISiteRepository>();
        _mockBuildingRepo = new Mock<IBuildingRepository>();
        _mockTenantContext = new Mock<ITenantContext>();

        _mockUnitOfWork.Setup(u => u.Sites).Returns(_mockSiteRepo.Object);
        _mockUnitOfWork.Setup(u => u.Buildings).Returns(_mockBuildingRepo.Object);
        _mockTenantContext.Setup(t => t.TenantId).Returns(Guid.NewGuid());

        _createHandler = new CreateBuildingCommandHandler(_mockUnitOfWork.Object, _mockTenantContext.Object);
    }

    [Fact]
    public async Task CreateBuilding_WhenValid_PersistsAndReturnsDto()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var site = new Site(siteId, _mockTenantContext.Object.TenantId, "Main Site", "SITE-01");
        var dto = new CreateBuildingDto(siteId, "Building 1", "BLD-01", "HQ Tower", "100 Main St");
        var command = new CreateBuildingCommand(dto);

        _mockSiteRepo.Setup(r => r.GetByIdAsync(siteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(site);
        _mockBuildingRepo.Setup(r => r.CodeExistsInSiteAsync(siteId, dto.Code, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _createHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Building 1", result.Name);
        Assert.Equal("BLD-01", result.Code);
        _mockBuildingRepo.Verify(r => r.AddAsync(It.IsAny<Building>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBuilding_WhenSiteNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var siteId = Guid.NewGuid();
        var dto = new CreateBuildingDto(siteId, "Building 1", "BLD-01", null, null);
        var command = new CreateBuildingCommand(dto);

        _mockSiteRepo.Setup(r => r.GetByIdAsync(siteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Site?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _createHandler.Handle(command, CancellationToken.None));
    }
}