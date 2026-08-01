using FluentValidation;
using RealEstateMarketplace.Application.DTOs;

namespace RealEstateMarketplace.Application.Validators;

public class CreateConversationValidator : AbstractValidator<CreateConversationDto>
{
    public CreateConversationValidator()
    {
        RuleFor(x => x.PropertyId)
            .GreaterThan(0).WithMessage("Property ID must be greater than 0.");

        RuleFor(x => x.BuyerId)
            .GreaterThan(0).WithMessage("Buyer ID must be greater than 0.");

        RuleFor(x => x.OwnerId)
            .GreaterThan(0).WithMessage("Owner ID must be greater than 0.");
    }
}

public class UpdateConversationValidator : AbstractValidator<UpdateConversationDto>
{
    public UpdateConversationValidator()
    {
        RuleFor(x => x.PropertyId)
            .GreaterThan(0).When(x => x.PropertyId.HasValue).WithMessage("Property ID must be greater than 0.");

        RuleFor(x => x.BuyerId)
            .GreaterThan(0).When(x => x.BuyerId.HasValue).WithMessage("Buyer ID must be greater than 0.");

        RuleFor(x => x.OwnerId)
            .GreaterThan(0).When(x => x.OwnerId.HasValue).WithMessage("Owner ID must be greater than 0.");
    }
}
