using FluentValidation;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Validators;

public class CreateMessageValidator : AbstractValidator<CreateMessageDto>
{
    public CreateMessageValidator()
    {
        RuleFor(x => x.ConversationId)
            .GreaterThan(0).WithMessage("Conversation ID must be greater than 0.");

        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Content is required.")
            .MaximumLength(2000).WithMessage("Content must not exceed 2000 characters.");
    }
}

public class UpdateMessageValidator : AbstractValidator<UpdateMessageDto>
{
    public UpdateMessageValidator()
    {
        RuleFor(x => x.Content)
            .MaximumLength(2000).WithMessage("Content must not exceed 2000 characters.");
    }
}
