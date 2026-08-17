using FluentValidation;

namespace Nms.Application.Locations.Rooms.Commands.UpdateRoom;

public class UpdateRoomCommandValidator : AbstractValidator<UpdateRoomCommand>
{
    public UpdateRoomCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Room Id is required.");

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Room name is required.")
            .MaximumLength(150).WithMessage("Room name must not exceed 150 characters.");

        RuleFor(x => x.Dto.Code)
            .NotEmpty().WithMessage("Room code is required.")
            .MaximumLength(50).WithMessage("Room code must not exceed 50 characters.");

        RuleFor(x => x.Dto.RoomType)
            .MaximumLength(100).WithMessage("RoomType must not exceed 100 characters.");
    }
}