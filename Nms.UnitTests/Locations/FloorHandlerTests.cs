using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Locations.Floors.Commands.CreateFloor;
using Nms.Application.Locations.Floors.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Locations;

public class FloorHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IBuildingRepository> _mockBuildingRepo;
    private readonly Mock<IFloorRepository> _mockFloorRepo;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly CreateFloorCommandHandler _createHandler;

    public FloorHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockBuildingRepo = new Mock<IBuildingRepository>();
        _mockFloorRepo = new Mock<IFloorRepository>();
        _mockTenantContext = new Mock<ITenantContext>();

        _mockUnitOfWork.Setup(u => u.Buildings).Returns(_mockBuildingRepo.Object);
        _mockUnitOfWork.Setup(u => u.Floors).Returns(_mockFloorRepo.Object);
        _mockTenantContext.Setup(t => t.TenantId).Returns(Guid.NewGuid());

        _createHandler = new CreateFloorCommandHandler(_mockUnitOfWork.Object, _mockTenantContext.Object);
    }

    [Fact]
    public async Task CreateFloor_WhenFloorNumberUnique_PersistsAndReturnsDto()
    {
        // Arrange
        var buildingId = Guid.NewGuid();
        var building = new Building(buildingId, _mockTenantContext.Object.TenantId, Guid.NewGuid(), "Building A", "BLD-A");
        var dto = new CreateFloorDto(buildingId, "Floor 2", 2, "Second Floor");
        var command = new CreateFloorCommand(dto);

        _mockBuildingRepo.Setup(r => r.GetByIdAsync(buildingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(building);
        _mockFloorRepo.Setup(r => r.FloorNumberExistsInBuildingAsync(buildingId, 2, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _createHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Floor 2", result.Name);
        Assert.Equal(2, result.FloorNumber);
        _mockFloorRepo.Verify(r => r.AddAsync(It.IsAny<Floor>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}