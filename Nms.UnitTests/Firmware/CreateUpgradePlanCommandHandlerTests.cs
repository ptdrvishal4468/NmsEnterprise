using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Firmware.Commands.CreateUpgradePlan;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Firmware;

public class CreateUpgradePlanCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IDeviceRepository> _deviceRepoMock = new();
    private readonly Mock<IFirmwareUpgradePlanRepository> _planRepoMock = new();
    private readonly Guid _tenantId = Guid.NewGuid();

    public CreateUpgradePlanCommandHandlerTests()
    {
        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _unitOfWorkMock.Setup(u => u.Devices).Returns(_deviceRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.FirmwareUpgradePlans).Returns(_planRepoMock.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldCreateUpgradePlan_WhenDeviceExists()
    {
        var deviceId = Guid.NewGuid();
        var device = new Device(deviceId, _tenantId, "Core-Router", "192.168.1.1", DeviceType.Router);

        _deviceRepoMock.Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(device);

        var command = new CreateUpgradePlanCommand(deviceId, "17.6.1", DateTime.UtcNow.AddDays(7), "Scheduled maintenance window");
        var handler = new CreateUpgradePlanCommandHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(deviceId, result.DeviceId);
        Assert.Equal("17.6.1", result.TargetVersion);
        Assert.Equal(UpgradePlanStatus.Planned, result.Status);

        _planRepoMock.Verify(r => r.AddAsync(It.IsAny<FirmwareUpgradePlan>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldThrowKeyNotFoundException_WhenDeviceNotFound()
    {
        var deviceId = Guid.NewGuid();
        _deviceRepoMock.Setup(r => r.GetByIdAsync(deviceId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Device?)null);

        var command = new CreateUpgradePlanCommand(deviceId, "17.6.1", DateTime.UtcNow.AddDays(7));
        var handler = new CreateUpgradePlanCommandHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}