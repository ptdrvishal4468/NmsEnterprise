using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Locations.Rooms.Commands.CreateRoom;
using Nms.Application.Locations.Rooms.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Locations;

public class RoomHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IFloorRepository> _mockFloorRepo;
    private readonly Mock<IRoomRepository> _mockRoomRepo;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly CreateRoomCommandHandler _createHandler;

    public RoomHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockFloorRepo = new Mock<IFloorRepository>();
        _mockRoomRepo = new Mock<IRoomRepository>();
        _mockTenantContext = new Mock<ITenantContext>();

        _mockUnitOfWork.Setup(u => u.Floors).Returns(_mockFloorRepo.Object);
        _mockUnitOfWork.Setup(u => u.Rooms).Returns(_mockRoomRepo.Object);
        _mockTenantContext.Setup(t => t.TenantId).Returns(Guid.NewGuid());

        _createHandler = new CreateRoomCommandHandler(_mockUnitOfWork.Object, _mockTenantContext.Object);
    }

    [Fact]
    public async Task CreateRoom_WhenValid_PersistsAndReturnsDto()
    {
        // Arrange
        var floorId = Guid.NewGuid();
        var floor = new Floor(floorId, _mockTenantContext.Object.TenantId, Guid.NewGuid(), "Floor 1", 1);
        var dto = new CreateRoomDto(floorId, "Server Room 101", "SR-101", "Datacenter", "Main server closet");
        var command = new CreateRoomCommand(dto);

        _mockFloorRepo.Setup(r => r.GetByIdAsync(floorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(floor);
        _mockRoomRepo.Setup(r => r.CodeExistsInFloorAsync(floorId, dto.Code, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _createHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Server Room 101", result.Name);
        Assert.Equal("SR-101", result.Code);
        _mockRoomRepo.Verify(r => r.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}