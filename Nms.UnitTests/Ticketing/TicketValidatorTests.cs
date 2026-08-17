using FluentValidation.TestHelper;
using Nms.Application.Ticketing.Commands.CreateTicket;
using Nms.Application.Ticketing.Commands.UpdateTicket;
using Nms.Domain.Enums;
using Xunit;

namespace Nms.UnitTests.Ticketing;

public class TicketValidatorTests
{
    private readonly CreateTicketCommandValidator _createValidator = new();
    private readonly UpdateTicketCommandValidator _updateValidator = new();

    [Fact]
    public void CreateTicket_WhenTitleIsEmpty_ShouldHaveValidationError()
    {
        var command = new CreateTicketCommand("", "Valid Description", TicketPriority.Medium, TicketingProviderType.ServiceNow);
        var result = _createValidator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void CreateTicket_WhenDescriptionIsEmpty_ShouldHaveValidationError()
    {
        var command = new CreateTicketCommand("Valid Title", "", TicketPriority.Medium, TicketingProviderType.ServiceNow);
        var result = _createValidator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void CreateTicket_WhenValid_ShouldNotHaveValidationErrors()
    {
        var command = new CreateTicketCommand("Core Router Down", "Detailed outage description", TicketPriority.Critical, TicketingProviderType.Jira);
        var result = _createValidator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateTicket_WhenIdIsEmpty_ShouldHaveValidationError()
    {
        var command = new UpdateTicketCommand(Guid.Empty, "Updated Title", "Updated Description", TicketPriority.High, TicketStatus.InProgress);
        var result = _updateValidator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}