using Moq;
using Nms.Application.Common.Interfaces;
using Nms.Application.Customers.Commands.AddCustomerContact;
using Nms.Application.Customers.Commands.CreateCustomer;
using Nms.Application.Customers.Commands.DeleteCustomer;
using Nms.Application.Customers.Commands.UpdateCustomer;
using Nms.Application.Customers.Dtos;
using Nms.Domain.Entities;
using Nms.Domain.Enums;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Customers;

public class CustomerCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<ICustomerContactRepository> _mockContactRepo;
    private readonly Mock<ITenantContext> _mockTenantContext;
    private readonly Guid _tenantId = Guid.NewGuid();

    public CustomerCommandHandlerTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockContactRepo = new Mock<ICustomerContactRepository>();
        _mockTenantContext = new Mock<ITenantContext>();

        _mockTenantContext.Setup(t => t.TenantId).Returns(_tenantId);
        _mockUow.Setup(u => u.Customers).Returns(_mockCustomerRepo.Object);
        _mockUow.Setup(u => u.CustomerContacts).Returns(_mockContactRepo.Object);
    }

    [Fact]
    public async Task CreateCustomer_ValidDto_CreatesAndReturnsDto()
    {
        var handler = new CreateCustomerCommandHandler(_mockUow.Object, _mockTenantContext.Object);
        var command = new CreateCustomerCommand(new CreateCustomerDto
        {
            Name = "Globex Corp",
            Code = "GLOBEX",
            Status = CustomerStatus.Active,
            Tier = CustomerTier.Enterprise
        });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Globex Corp", result.Name);
        Assert.Equal("GLOBEX", result.Code);
        Assert.Equal(_tenantId, result.TenantId);
        _mockCustomerRepo.Verify(r => r.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomer_CircularHierarchyDetected_ThrowsInvalidOperationException()
    {
        var customerId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var customer = new Customer(customerId, _tenantId, "Child Corp", "CHILD");

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockCustomerRepo.Setup(r => r.GetByIdAsync(parentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Customer(parentId, _tenantId, "Parent Corp", "PARENT"));
        _mockCustomerRepo.Setup(r => r.HasCircularHierarchyAsync(customerId, parentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new UpdateCustomerCommandHandler(_mockUow.Object);
        var command = new UpdateCustomerCommand(customerId, new UpdateCustomerDto
        {
            Name = "Child Corp",
            Code = "CHILD",
            ParentCustomerId = parentId
        });

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCustomer_HasChildAccounts_ThrowsInvalidOperationException()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer(customerId, _tenantId, "Parent Corp", "PARENT");
        var child = new Customer(Guid.NewGuid(), _tenantId, "Child Corp", "CHILD", parentCustomerId: customerId);

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockCustomerRepo.Setup(r => r.GetChildrenAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Customer> { child });

        var handler = new DeleteCustomerCommandHandler(_mockUow.Object);
        var command = new DeleteCustomerCommand(customerId);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task AddCustomerContact_DesignatedAsPrimary_DemotesExistingPrimaryContact()
    {
        var customerId = Guid.NewGuid();
        var customer = new Customer(customerId, _tenantId, "Acme Corp", "ACME");
        var existingPrimary = new CustomerContact(Guid.NewGuid(), _tenantId, customerId, "Jane", "Doe", "jane@example.com", isPrimary: true);

        _mockCustomerRepo.Setup(r => r.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockContactRepo.Setup(r => r.GetPrimaryContactAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPrimary);

        var handler = new AddCustomerContactCommandHandler(_mockUow.Object, _mockTenantContext.Object);
        var command = new AddCustomerContactCommand(customerId, new CustomerContactInputDto
        {
            FirstName = "John",
            LastName = "Smith",
            Email = "john.smith@example.com",
            IsPrimary = true
        });

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.IsPrimary);
        Assert.False(existingPrimary.IsPrimary);
        _mockContactRepo.Verify(r => r.Update(existingPrimary), Times.Once);
        _mockContactRepo.Verify(r => r.AddAsync(It.IsAny<CustomerContact>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}