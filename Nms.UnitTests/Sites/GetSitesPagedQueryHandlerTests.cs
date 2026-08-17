using Moq;
using Nms.Application.Sites.Queries.GetSitesPaged;
using Nms.Domain.Entities;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Sites;

public class GetSitesPagedQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ISiteRepository> _mockSiteRepo;
    private readonly GetSitesPagedQueryHandler _handler;

    public GetSitesPagedQueryHandlerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockSiteRepo = new Mock<ISiteRepository>();
        _mockUnitOfWork.Setup(u => u.Sites).Returns(_mockSiteRepo.Object);

        _handler = new GetSitesPagedQueryHandler(_mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_WithSearchTerm_ReturnsFilteredPagedResult()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var sites = new List<Site>
        {
            new(Guid.NewGuid(), tenantId, "Dallas DC", "DAL-01", city: "Dallas"),
            new(Guid.NewGuid(), tenantId, "Austin Facility", "AUS-01", city: "Austin"),
            new(Guid.NewGuid(), tenantId, "Frankfurt DC", "FRA-01", city: "Frankfurt")
        };

        _mockSiteRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(sites);

        var query = new GetSitesPagedQuery(1, 10, SearchTerm: "DC");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count);
        Assert.All(result.Items, item => Assert.Contains("DC", item.Name));
    }
}