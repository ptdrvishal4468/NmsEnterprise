using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Firmware.Commands.CreateFirmwareBaseline;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Firmware;

public class CreateFirmwareBaselineCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly Mock<IFirmwareBaselineRepository> _baselineRepoMock = new();
    private readonly Guid _tenantId = Guid.NewGuid();

    public CreateFirmwareBaselineCommandHandlerTests()
    {
        _tenantContextMock.Setup(t => t.TenantId).Returns(_tenantId);
        _unitOfWorkMock.Setup(u => u.FirmwareBaselines).Returns(_baselineRepoMock.Object);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task Handle_ShouldCreateNewBaseline_WhenNoneExists()
    {
        var command = new CreateFirmwareBaselineCommand("Cisco", "Catalyst 9300", "17.3.4", "Approved release");

        _baselineRepoMock.Setup(r => r.GetByVendorAndModelAsync(_tenantId, "Cisco", "Catalyst 9300", It.IsAny<CancellationToken>()))
            .ReturnsAsync((FirmwareBaseline?)null);

        var handler = new CreateFirmwareBaselineCommandHandler(_unitOfWorkMock.Object, _tenantContextMock.Object);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Cisco", result.Vendor);
        Assert.Equal("Catalyst 9300", result.Model);
        Assert.Equal("17.3.4", result.TargetVersion);

        _baselineRepoMock.Verify(r => r.AddAsync(It.IsAny<FirmwareBaseline>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}