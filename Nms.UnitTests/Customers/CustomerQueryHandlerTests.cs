using Moq;
using Nms.Application.Customers.Dtos;
using Nms.Application.Customers.Queries.GetCustomerById;
using Nms.Application.Customers.Queries.GetCustomerContacts;
using Nms.Application.Customers.Queries.GetCustomerHierarchy;
using Nms.Application.Customers.Queries.GetCustomersPaged;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Customers;

public class CustomerQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<ICustomerContactRepository> _mockContactRepo;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CustomerQueryHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockContactRepo = new Mock<ICustomerContactRepository>();

        _mockUow.Setup(u => u.Customers).Returns(_mockCustomerRepo.Object);
        _mockUow.Setup(u => u.CustomerContacts).Returns(_mockContactRepo.Object);
    }

    [Fact]
    public async Task GetCustomerById_Exists_ReturnsCustomerDtoWithCounts()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer(customerId, _tenantId, "Wayne Enterprises", "WAYNE");
        var contact = new CustomerContact(Guid.NewGuid(), _tenantId, customerId, "Bruce", "Wayne", "bruce@wayne.com");
        var child = new Customer(Guid.NewGuid(), _tenantId, "Wayne Aerospace", "AERO", parentCustomerId: customerId);

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockContactRepo.Setup(r => r.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CustomerContact> { contact });
        _mockCustomerRepo.Setup(r => r.GetChildrenAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer> { child });

        var handler = new GetCustomerByIdQueryHandler(_mockUow.Object);
        var result = await handler.Handle(new GetCustomerByIdQuery(customerId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(customerId, result.Id);
        Assert.Equal("Wayne Enterprises", result.Name);
        Assert.Equal(1, result.ContactCount);
        Assert.Equal(1, result.ChildCustomerCount);
    }

    [Fact]
    public async Task GetCustomerHierarchy_ConstructsNestedTreeStructure()
    {
        var rootId = Guid.NewGuid();
        var childId = Guid.NewGuid();
        var root = new Customer(rootId, _tenantId, "Holding Corp", "HOLDING");
        var child = new Customer(childId, _tenantId, "Subsidiary A", "SUB-A", parentCustomerId: rootId);

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(rootId, It.IsAny<CancellationToken>())).ReturnsAsync(root);
        _mockCustomerRepo.Setup(r => r.GetByIdAsync(childId, It.IsAny<CancellationToken>())).ReturnsAsync(child);
        _mockCustomerRepo.Setup(r => r.GetChildrenAsync(rootId, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Customer> { child });
        _mockCustomerRepo.Setup(r => r.GetChildrenAsync(childId, It.IsAny<CancellationToken>())).ReturnsAsync(new List<Customer>());

        var handler = new GetCustomerHierarchyQueryHandler(_mockUow.Object);
        var result = await handler.Handle(new GetCustomerHierarchyQuery(rootId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(rootId, result.Id);
        Assert.Single(result.Children);
        Assert.Equal(childId, result.Children[0].Id);
    }

    [Fact]
    public async Task GetCustomerContacts_ReturnsSortedWithPrimaryFirst()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer(customerId, _tenantId, "Stark Industries", "STARK");
        var secondaryContact = new CustomerContact(Guid.NewGuid(), _tenantId, customerId, "Pepper", "Potts", "pepper@stark.com", isPrimary: false);
        var primaryContact = new CustomerContact(Guid.NewGuid(), _tenantId, customerId, "Tony", "Stark", "tony@stark.com", isPrimary: true);

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>())).ReturnsAsync(customer);
        _mockContactRepo.Setup(r => r.GetByCustomerIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CustomerContact> { secondaryContact, primaryContact });

        var handler = new GetCustomerContactsQueryHandler(_mockUow.Object);
        var result = await handler.Handle(new GetCustomerContactsQuery(customerId), CancellationToken.None);

        Assert.Equal(2, result.Count);
        Assert.True(result[0].IsPrimary);
        Assert.Equal("Tony", result[0].FirstName);
    }
}