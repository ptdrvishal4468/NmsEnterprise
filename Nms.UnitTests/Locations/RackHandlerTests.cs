using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Locations.Racks.Commands.CreateRack;
using Nms.Application.Locations.Racks.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Locations;

public class RackHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IRoomRepository> _mockRoomRepo;
    private readonly Mock<IRackRepository> _mockRackRepo;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly CreateRackCommandHandler _createHandler;

    public RackHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockRoomRepo = new Mock<IRoomRepository>();
        _mockRackRepo = new Mock<IRackRepository>();
        _mockTenantContext = new Mock<ITenantContext>();

        _mockUnitOfWork.Setup(u => u.Rooms).Returns(_mockRoomRepo.Object);
        _mockUnitOfWork.Setup(u => u.Racks).Returns(_mockRackRepo.Object);
        _mockTenantContext.Setup(t => t.TenantId).Returns(Guid.NewGuid());

        _createHandler = new CreateRackCommandHandler(_mockUnitOfWork.Object, _mockTenantContext.Object);
    }

    [Fact]
    public async Task CreateRack_WhenValid_PersistsAndReturnsDto()
    {
        // Arrange
        var roomId = Guid.NewGuid();
        var room = new Room(roomId, _mockTenantContext.Object.TenantId, Guid.NewGuid(), "MDF Room", "MDF-01");
        var dto = new CreateRackDto(roomId, "Core Switch Rack", "RACK-A01", 42, 19, 1000, 5000, 1000, "Row 1");
        var command = new CreateRackCommand(dto);

        _mockRoomRepo.Setup(r => r.GetByIdAsync(roomId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _mockRackRepo.Setup(r => r.IdentifierExistsInRoomAsync(roomId, dto.Identifier, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _createHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Core Switch Rack", result.Name);
        Assert.Equal("RACK-A01", result.Identifier);
        Assert.Equal(42, result.HeightInUnits);
        _mockRackRepo.Verify(r => r.AddAsync(It.IsAny<Rack>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Rack_WithZeroOrNegativeHeight_ThrowsArgumentOutOfRangeException()
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => new Rack(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Invalid Rack",
            "RACK-01",
            heightInUnits: 0));
    }
}