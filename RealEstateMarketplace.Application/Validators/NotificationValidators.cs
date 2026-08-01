using FluentValidation;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Validators;

public class CreateNotificationValidator : AbstractValidator<CreateNotificationDto>
{
    public CreateNotificationValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150).WithMessage("Title must not exceed 150 characters.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .MaximumLength(1000).WithMessage("Message must not exceed 1000 characters.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("User ID must be greater than 0.");
    }
}

public class UpdateNotificationValidator : AbstractValidator<UpdateNotificationDto>
{
    public UpdateNotificationValidator()
    {
        RuleFor(x => x.Title)
            .MaximumLength(150).WithMessage("Title must not exceed 150 characters.");

        RuleFor(x => x.Message)
            .MaximumLength(1000).WithMessage("Message must not exceed 1000 characters.");
    }
}
