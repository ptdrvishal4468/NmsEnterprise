using Nms.Application.Common.Interfaces;
using Nms.Domain.Enums;
using Nms.Infrastructure.Connectivity;
using Moq;
using Xunit;

namespace Nms.UnitTests;

public class ConnectionAdapterFactoryTests
{
    [Fact]
    public void GetAdapter_ShouldReturnMatchingAdapter_WhenProtocolIsRegistered()
    {
        // Arrange
        var mockIcmpAdapter = new Mock<IConnectionAdapter>();
        mockIcmpAdapter.Setup(a => a.Protocol).Returns(NetworkProtocol.Icmp);

        var adapters = new List<IConnectionAdapter> { mockIcmpAdapter.Object };
        var factory = new ConnectionAdapterFactory(adapters);

        // Act
        var result = factory.GetAdapter(NetworkProtocol.Icmp);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(NetworkProtocol.Icmp, result.Protocol);
    }

    [Fact]
    public void GetAdapter_ShouldThrowNotSupportedException_WhenProtocolIsNotRegistered()
    {
        // Arrange
        var adapters = new List<IConnectionAdapter>();
        var factory = new ConnectionAdapterFactory(adapters);

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => factory.GetAdapter(NetworkProtocol.Ssh));
    }
}