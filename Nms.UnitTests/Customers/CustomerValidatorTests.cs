using Moq;
using Nms.Application.Customers.Commands.AddCustomerContact;
using Nms.Application.Customers.Commands.CreateCustomer;
using Nms.Application.Customers.Dtos;
using Nms.Domain.Interfaces;
using Xunit;

namespace Nms.UnitTests.Customers;

public class CustomerValidatorTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<ICustomerRepository> _mockCustomerRepo;
    private readonly Mock<ICustomerContactRepository> _mockContactRepo;

    public CustomerValidatorTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockContactRepo = new Mock<ICustomerContactRepository>();

        _mockUow.Setup(u => u.Customers).Returns(_mockCustomerRepo.Object);
        _mockUow.Setup(u => u.CustomerContacts).Returns(_mockContactRepo.Object);
    }

    [Fact]
    public async Task CreateCustomerCommandValidator_DuplicateCode_FailsValidation()
    {
        _mockCustomerRepo.Setup(r => r.CodeExistsAsync("DUPLICATE", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateCustomerCommandValidator(_mockUow.Object);
        var command = new CreateCustomerCommand(new CreateCustomerDto
        {
            Name = "Sample Corp",
            Code = "DUPLICATE"
        });

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Dto.Code");
    }

    [Fact]
    public async Task AddCustomerContactCommandValidator_InvalidEmail_FailsValidation()
    {
        var validator = new AddCustomerContactCommandValidator(_mockUow.Object);
        var command = new AddCustomerContactCommand(Guid.NewGuid(), new CustomerContactInputDto
        {
            FirstName = "Alice",
            LastName = "Smith",
            Email = "not-a-valid-email"
        });

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Dto.Email");
    }
}